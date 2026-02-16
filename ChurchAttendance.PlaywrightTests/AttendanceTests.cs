using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AttendanceTests : PlaywrightTestBase
{
    private async Task EnsureTestMemberExists(string name, string ageGroup = "Men")
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(name);
        await Page.Locator("#ageGroup").SelectOptionAsync(ageGroup);
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-sections")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePage_LoadsChecklistForToday()
    {
        var memberName = $"AttendMember_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");

        // Wait for the attendance checklist to load via HTMX
        await Expect(Page.Locator("#attendance-area .age-group-section")).ToHaveCountAsync(
            await Page.Locator("#attendance-area .age-group-section").CountAsync(),
            new() { Timeout = 5000 });

        // Should show our test member in the checklist
        await Expect(Page.Locator("#attendance-area")).ToContainTextAsync(memberName);
    }

    [Test]
    public async Task AttendancePage_DateDefaultsToToday()
    {
        await Page.GotoAsync($"{BaseUrl}/attendance");

        var today = DateTime.Today.ToString("yyyy-MM-dd");
        await Expect(Page.Locator("#date")).ToHaveValueAsync(today);
    }

    [Test]
    public async Task AttendancePage_CheckboxToggles_UpdatesCounter()
    {
        var memberName = $"CounterMember_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area .age-group-section").First).ToBeVisibleAsync();

        // Expand the section containing our member and check the checkbox
        var row = Page.Locator($".attendance-row[data-name*='{memberName.ToLowerInvariant()[..15]}']");
        // Click the section header to expand it first
        var section = row.Locator("xpath=ancestor::div[contains(@class,'age-group-section')]");
        await section.Locator(".age-group-header").ClickAsync();

        var checkbox = row.Locator("input[name='memberIds']");
        await checkbox.CheckAsync();

        // The top bar counter should update to show at least 1 present
        var counter = Page.Locator("#top-bar-count");
        await Expect(counter).ToContainTextAsync("present");
    }

    [Test]
    public async Task AttendancePage_AutoSave_FiresOnCheckboxChange()
    {
        var memberName = $"AutoSave_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area .age-group-section").First).ToBeVisibleAsync();

        var row = Page.Locator($".attendance-row[data-name*='{memberName.ToLowerInvariant()[..15]}']");
        var section = row.Locator("xpath=ancestor::div[contains(@class,'age-group-section')]");
        await section.Locator(".age-group-header").ClickAsync();

        var checkbox = row.Locator("input[name='memberIds']");
        await checkbox.CheckAsync();

        // Wait for auto-save status to show
        var status = Page.Locator("#auto-save-status");
        await Expect(status).Not.ToBeEmptyAsync();
        await Expect(status).ToContainTextAsync(new Regex("Sav(ed|ing)"), new() { Timeout = 5000 });
    }

    [Test]
    public async Task AttendancePage_NameFilter_FiltersRows()
    {
        var memberA = $"FilterAlpha_{Guid.NewGuid():N}"[..25];
        var memberB = $"FilterBravo_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberA);
        await EnsureTestMemberExists(memberB);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area .age-group-section").First).ToBeVisibleAsync();

        // Open the search input
        await Page.Locator("#search-toggle").ClickAsync();
        await Expect(Page.Locator("#name-filter")).ToBeVisibleAsync();

        // Type in the name filter to filter for memberA
        await Page.Locator("#name-filter").FillAsync("FilterAlpha");

        // memberA row should be visible, memberB row should be hidden
        var rowA = Page.Locator($".attendance-row[data-name='{memberA.ToLowerInvariant()}']");
        var rowB = Page.Locator($".attendance-row[data-name='{memberB.ToLowerInvariant()}']");
        await Expect(rowA).ToBeVisibleAsync();
        await Expect(rowB).ToBeHiddenAsync();
    }

    [Test]
    public async Task AttendancePage_ViewSummary_ShowsSummary()
    {
        var memberName = $"Summary_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area .age-group-section").First).ToBeVisibleAsync();

        // Expand and check a member
        var row = Page.Locator($".attendance-row[data-name*='{memberName.ToLowerInvariant()[..12]}']");
        var section = row.Locator("xpath=ancestor::div[contains(@class,'age-group-section')]");
        await section.Locator(".age-group-header").ClickAsync();
        await row.Locator("input[name='memberIds']").CheckAsync();

        // Click "View Summary"
        await Page.GetByRole(AriaRole.Button, new() { Name = "View Summary" }).ClickAsync();

        // Wait for summary to appear
        await Expect(Page.GetByText("Attendance Summary")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Total Present")).ToBeVisibleAsync();
        await Expect(Page.GetByText("By Age Group")).ToBeVisibleAsync();
        await Expect(Page.GetByText("By Category")).ToBeVisibleAsync();

        // Success message
        await Expect(Page.GetByText("Attendance saved successfully!")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePage_HasStickyTopBar()
    {
        var memberName = $"TopBar_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-top-bar")).ToBeVisibleAsync();

        // Top bar has count and search toggle
        await Expect(Page.Locator("#top-bar-count")).ToBeVisibleAsync();
        await Expect(Page.Locator("#search-toggle")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePage_HasCollapsibleSections()
    {
        var memberName = $"Collapse_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        var sections = Page.Locator("#attendance-area .age-group-section");
        await Expect(sections.First).ToBeVisibleAsync();

        // Sections should default to collapsed
        var firstSection = sections.First;
        await Expect(firstSection).ToHaveClassAsync(new Regex("collapsed"));

        // Click header to expand
        await firstSection.Locator(".age-group-header").ClickAsync();
        await Expect(firstSection).Not.ToHaveClassAsync(new Regex("collapsed"));

        // Click again to collapse
        await firstSection.Locator(".age-group-header").ClickAsync();
        await Expect(firstSection).ToHaveClassAsync(new Regex("collapsed"));
    }
}
