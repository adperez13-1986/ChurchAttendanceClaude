using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AttendanceTests : PlaywrightTestBase
{
    private async Task EnsureTestMemberExists(string name)
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();

        await Page.Locator("#fullName").FillAsync(name);
        await Page.Locator("#ageGroup").SelectOptionAsync("Men");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(name);
    }

    [Test]
    public async Task AttendancePage_LoadsChecklistForToday()
    {
        // Ensure at least one member exists
        var memberName = $"AttendMember_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");

        // Wait for the attendance checklist to load via HTMX
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

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
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        // Find the row with the test member and check the checkbox
        var row = Page.Locator($"tr[data-name*='{memberName.ToLowerInvariant()[..15]}']");
        var checkbox = row.Locator("input[name='memberIds']");
        await checkbox.CheckAsync();

        // The counter should update to show at least 1 present
        var counter = Page.Locator("#attendance-count");
        await Expect(counter).ToContainTextAsync("present");
    }

    [Test]
    public async Task AttendancePage_AutoSave_FiresOnCheckboxChange()
    {
        var memberName = $"AutoSave_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        var row = Page.Locator($"tr[data-name*='{memberName.ToLowerInvariant()[..15]}']");
        var checkbox = row.Locator("input[name='memberIds']");
        await checkbox.CheckAsync();

        // Wait for auto-save status to show
        var status = Page.Locator("#auto-save-status");
        await Expect(status).Not.ToBeEmptyAsync();
        // Should eventually show "Saved" message
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
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        // Both members should be visible initially
        await Expect(Page.Locator($"tr[data-name*='{memberA.ToLowerInvariant()[..12]}']")).ToBeVisibleAsync();
        await Expect(Page.Locator($"tr[data-name*='{memberB.ToLowerInvariant()[..12]}']")).ToBeVisibleAsync();

        // Type in the name filter to filter for memberA
        await Page.Locator("#name-filter").FillAsync("FilterAlpha");

        // memberA row should be visible, memberB row should be hidden
        await Expect(Page.Locator($"tr[data-name*='{memberA.ToLowerInvariant()[..12]}']")).ToBeVisibleAsync();
        await Expect(Page.Locator($"tr[data-name*='{memberB.ToLowerInvariant()[..12]}']")).ToBeHiddenAsync();
    }

    [Test]
    public async Task AttendancePage_AgeGroupFilter_FiltersRows()
    {
        var memberMen = $"AgeMen_{Guid.NewGuid():N}"[..25];
        var memberWomen = $"AgeWomen_{Guid.NewGuid():N}"[..25];

        // Create a Men member
        await Page.GotoAsync($"{BaseUrl}/members");
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();
        await Page.Locator("#fullName").FillAsync(memberMen);
        await Page.Locator("#ageGroup").SelectOptionAsync("Men");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(memberMen);

        // Create a Women member
        await Page.GetByText("Add New Member").ClickAsync();
        await Expect(Page.Locator("#member-form-area form")).ToBeVisibleAsync();
        await Page.Locator("#fullName").FillAsync(memberWomen);
        await Page.Locator("#ageGroup").SelectOptionAsync("Women");
        await Page.Locator("#category").SelectOptionAsync("Member");
        await Page.Locator("#member-form-area form button[type='submit']").ClickAsync();
        await Expect(Page.Locator("#members-table")).ToContainTextAsync(memberWomen);

        // Go to attendance page
        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        // Filter by "Women" age group
        await Page.Locator("#age-group-filter").SelectOptionAsync("Women");

        // Men member should be hidden, Women member should be visible
        var menRow = Page.Locator($"tr[data-name*='{memberMen.ToLowerInvariant()[..10]}']");
        var womenRow = Page.Locator($"tr[data-name*='{memberWomen.ToLowerInvariant()[..10]}']");
        await Expect(menRow).ToBeHiddenAsync();
        await Expect(womenRow).ToBeVisibleAsync();

        // Reset filter to "All"
        await Page.Locator("#age-group-filter").SelectOptionAsync("");
        await Expect(menRow).ToBeVisibleAsync();
        await Expect(womenRow).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePage_SelectAll_ChecksAllVisibleMembers()
    {
        var memberName = $"SelectAll_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        // Click select-all checkbox
        await Page.Locator("#select-all").CheckAsync();

        // All visible checkboxes should be checked
        var checkboxes = Page.Locator("input[name='memberIds']");
        var count = await checkboxes.CountAsync();
        Assert.That(count, Is.GreaterThan(0));

        for (int i = 0; i < count; i++)
        {
            var cb = checkboxes.Nth(i);
            var row = cb.Locator("xpath=ancestor::tr[@data-age-group]");
            var isVisible = await row.IsVisibleAsync();
            if (isVisible)
            {
                await Expect(cb).ToBeCheckedAsync();
            }
        }

        // Uncheck select-all
        await Page.Locator("#select-all").UncheckAsync();

        // All should be unchecked
        for (int i = 0; i < count; i++)
        {
            var cb = checkboxes.Nth(i);
            var row = cb.Locator("xpath=ancestor::tr[@data-age-group]");
            var isVisible = await row.IsVisibleAsync();
            if (isVisible)
            {
                await Expect(cb).Not.ToBeCheckedAsync();
            }
        }
    }

    [Test]
    public async Task AttendancePage_ViewSummary_ShowsSummary()
    {
        var memberName = $"Summary_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        // Check a member
        var row = Page.Locator($"tr[data-name*='{memberName.ToLowerInvariant()[..12]}']");
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
    public async Task AttendancePage_ServiceTypeAutoDetect_Sunday()
    {
        await Page.GotoAsync($"{BaseUrl}/attendance");

        // Find a Sunday date
        var today = DateTime.Today;
        var nextSunday = today.AddDays((7 - (int)today.DayOfWeek) % 7);
        if (nextSunday > today)
            nextSunday = nextSunday.AddDays(-7); // Get the most recent Sunday in the past

        // If today is Sunday, use today; otherwise find a recent Sunday that won't be blocked by max date
        var sundayDate = today.DayOfWeek == DayOfWeek.Sunday ? today : nextSunday;

        // Only test if the Sunday date is not in the future (max is today)
        if (sundayDate <= today)
        {
            await Page.Locator("#date").FillAsync(sundayDate.ToString("yyyy-MM-dd"));
            // Trigger change event
            await Page.Locator("#date").DispatchEventAsync("change");

            var serviceType = await Page.Locator("#serviceType").InputValueAsync();
            Assert.That(serviceType, Is.EqualTo("SundayService"));
        }
    }

    [Test]
    public async Task AttendancePage_HasSearchAndFilterControls()
    {
        var memberName = $"Controls_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        // Name filter
        await Expect(Page.Locator("#name-filter")).ToBeVisibleAsync();
        // Age group filter
        await Expect(Page.Locator("#age-group-filter")).ToBeVisibleAsync();
        // Select-all checkbox
        await Expect(Page.Locator("#select-all")).ToBeVisibleAsync();
        // Counter
        await Expect(Page.Locator("#attendance-count")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePage_AgeGroupFilter_HasAllOptions()
    {
        var memberName = $"FilterOpts_{Guid.NewGuid():N}"[..25];
        await EnsureTestMemberExists(memberName);

        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("#attendance-area table")).ToBeVisibleAsync();

        var ageFilter = Page.Locator("#age-group-filter");
        var options = ageFilter.Locator("option");

        // "All" + 6 age groups = 7 options
        await Expect(options).ToHaveCountAsync(7);

        var texts = await options.AllTextContentsAsync();
        Assert.That(texts, Does.Contain("All"));
        Assert.That(texts, Does.Contain("Men"));
        Assert.That(texts, Does.Contain("Women"));
        Assert.That(texts, Does.Contain("YAN"));
        Assert.That(texts, Does.Contain("CYN"));
        Assert.That(texts, Does.Contain("Children"));
        Assert.That(texts, Does.Contain("Infants"));
    }
}
