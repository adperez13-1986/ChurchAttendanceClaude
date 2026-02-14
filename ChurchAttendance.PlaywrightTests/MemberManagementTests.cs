using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MemberManagementTests : PlaywrightTestBase
{
    private string _testMemberName = null!;

    [SetUp]
    public void SetUp()
    {
        // Generate a unique name per test to avoid collisions
        _testMemberName = $"TestMember_{Guid.NewGuid():N}"[..30];
    }

    [Test]
    public async Task AddNewMember_OpensModal()
    {
        await Page.GotoAsync($"{BaseUrl}/members");

        // Modal overlay should not be visible initially
        var overlay = Page.Locator("#member-modal-overlay");
        await Expect(overlay).Not.ToBeVisibleAsync();

        // Click "Add New Member" button
        await Page.GetByText("Add New Member").ClickAsync();

        // Wait for the form to load via HTMX
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        // Modal overlay should now be visible
        await Expect(overlay).ToBeVisibleAsync();

        // Modal title should say "Add New Member"
        await Expect(Page.Locator("#modal-title")).ToHaveTextAsync("Add New Member");
    }

    [Test]
    public async Task AddNewMember_FormHasRequiredFields()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        // Check form fields exist
        await Expect(Page.Locator("#fullName")).ToBeVisibleAsync();
        await Expect(Page.Locator("#ageGroup")).ToBeVisibleAsync();
        await Expect(Page.Locator("#category")).ToBeVisibleAsync();
        await Expect(Page.Locator("#firstAttendedDate")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AddNewMember_AgeGroupDropdown_HasAllOptions()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        var ageGroupSelect = Page.Locator("#ageGroup");
        var options = ageGroupSelect.Locator("option");

        // Should have 6 age groups: Men, Women, YAN, CYN, Children, Infants
        await Expect(options).ToHaveCountAsync(6);

        var optionTexts = await options.AllTextContentsAsync();
        Assert.That(optionTexts, Does.Contain("Men"));
        Assert.That(optionTexts, Does.Contain("Women"));
        Assert.That(optionTexts, Does.Contain("YAN"));
        Assert.That(optionTexts, Does.Contain("CYN"));
        Assert.That(optionTexts, Does.Contain("Children"));
        Assert.That(optionTexts, Does.Contain("Infants"));
    }

    [Test]
    public async Task AddNewMember_CategoryDropdown_HasAllOptions()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        var categorySelect = Page.Locator("#category");
        var options = categorySelect.Locator("option");

        // Should have 3 categories: Member, Visitor, Under Monitoring
        await Expect(options).ToHaveCountAsync(3);

        var optionTexts = await options.AllTextContentsAsync();
        Assert.That(optionTexts, Does.Contain("Member"));
        Assert.That(optionTexts, Does.Contain("Visitor"));
        Assert.That(optionTexts, Does.Contain("Under Monitoring"));
    }

    [Test]
    public async Task AddNewMember_SubmitForm_MemberAppearsInTable()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        // Fill the form
        await Page.Locator("#fullName").FillAsync(_testMemberName);
        await Page.Locator("#ageGroup").SelectOptionAsync("Men");
        await Page.Locator("#category").SelectOptionAsync("Member");

        // Submit
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();

        // Wait for table to update via HTMX
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(_testMemberName);

        // The modal should close
        var overlay = Page.Locator("#member-modal-overlay");
        await Expect(overlay).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task AddNewMember_AsVisitor_ShowsCorrectCategory()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(_testMemberName);
        await Page.Locator("#ageGroup").SelectOptionAsync("Women");
        await Page.Locator("#category").SelectOptionAsync("Visitor");

        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();

        // Wait for table to update
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(_testMemberName);

        // Find the row with this member and verify category
        var row = Page.Locator("#members-table tbody tr", new() { HasText = _testMemberName });
        await Expect(row).ToContainTextAsync("Visitor");
        await Expect(row).ToContainTextAsync("Women");
    }

    [Test]
    public async Task AddNewMember_WithFirstAttendedDate()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(_testMemberName);
        await Page.Locator("#ageGroup").SelectOptionAsync("YAN");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#firstAttendedDate").FillAsync(DateTime.Today.ToString("yyyy-MM-dd"));

        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();

        await Expect(Page.Locator("#members-table")).ToContainTextAsync(_testMemberName);
    }

    [Test]
    public async Task EditMember_OpensModalWithExistingData()
    {
        // First, create a member
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(_testMemberName);
        await Page.Locator("#ageGroup").SelectOptionAsync("CYN");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(_testMemberName);

        // Now click Edit on that member
        var row = Page.Locator("#members-table tbody tr", new() { HasText = _testMemberName });
        await row.GetByText("Edit").ClickAsync();

        // Wait for the edit form to load
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();
        await Expect(Page.Locator("#modal-title")).ToHaveTextAsync("Edit Member");

        // Verify existing data is populated
        await Expect(Page.Locator("#fullName")).ToHaveValueAsync(_testMemberName);
    }

    [Test]
    public async Task EditMember_UpdateName_ReflectsInTable()
    {
        // Create a member
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(_testMemberName);
        await Page.Locator("#ageGroup").SelectOptionAsync("Men");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(_testMemberName);

        // Edit the member
        var row = Page.Locator("#members-table tbody tr", new() { HasText = _testMemberName });
        await row.GetByText("Edit").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        var updatedName = _testMemberName + " Updated";
        await Page.Locator("#fullName").ClearAsync();
        await Page.Locator("#fullName").FillAsync(updatedName);
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();

        // Verify updated name appears in table
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(updatedName);
    }

    [Test]
    public async Task DeactivateMember_RemovesActiveStatus()
    {
        // Create a member
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(_testMemberName);
        await Page.Locator("#ageGroup").SelectOptionAsync("Men");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(_testMemberName);

        // Verify the member is Active
        var row = Page.Locator("#members-table tbody tr", new() { HasText = _testMemberName });
        await Expect(row).ToContainTextAsync("Active");

        // Accept the confirmation dialog
        Page.Dialog += (_, dialog) => dialog.AcceptAsync();

        // Click Deactivate
        await row.GetByText("Deactivate").ClickAsync();

        // Wait for table refresh. The member should now show Inactive
        var updatedRow = Page.Locator("#members-table tbody tr", new() { HasText = _testMemberName });
        await Expect(updatedRow).ToContainTextAsync("Inactive");
    }

    [Test]
    public async Task CancelButton_ClosesModal()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        var overlay = Page.Locator("#member-modal-overlay");
        await Expect(overlay).ToBeVisibleAsync();

        // Click Cancel
        await Page.GetByText("Cancel").ClickAsync();
        await Expect(overlay).Not.ToBeVisibleAsync();
    }
}
