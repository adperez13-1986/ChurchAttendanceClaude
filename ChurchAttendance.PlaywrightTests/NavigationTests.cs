using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class NavigationTests : PlaywrightTestBase
{
    [Test]
    public async Task DashboardLoads_WithTitle()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page).ToHaveTitleAsync(new Regex("Church Attendance"));
    }

    [Test]
    public async Task DashboardLoads_WithStatCards()
    {
        await Page.GotoAsync(BaseUrl);

        // Three stat cards: Total Members, Active Members, Today's Attendance
        var stats = Page.Locator(".stat");
        await Expect(stats).ToHaveCountAsync(3);
        await Expect(stats.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task DashboardLoads_WithHeaders()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Total Members")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Active Members")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Today's Attendance")).ToBeVisibleAsync();
    }

    [Test]
    public async Task MembersPageLoads()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Expect(Page).ToHaveTitleAsync(new Regex("Members"));
        await Expect(Page.GetByText("Add New Member")).ToBeVisibleAsync();
    }

    [Test]
    public async Task MembersPage_HasTable()
    {
        await Page.GotoAsync($"{BaseUrl}/members");
        await Expect(Page.Locator("#members-table")).ToBeVisibleAsync();

        // Table headers
        await Expect(Page.GetByRole(AriaRole.Columnheader, new() { Name = "Name" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Columnheader, new() { Name = "Age Group" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Columnheader, new() { Name = "Category" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Columnheader, new() { Name = "Status" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Columnheader, new() { Name = "Actions" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePageLoads()
    {
        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page).ToHaveTitleAsync(new Regex("Attendance"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Attendance" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task AttendancePage_HasDateInput()
    {
        await Page.GotoAsync($"{BaseUrl}/attendance");
        var dateInput = Page.Locator("#date");
        await Expect(dateInput).ToBeVisibleAsync();

        // Default date is today
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        await Expect(dateInput).ToHaveValueAsync(today);
    }

    [Test]
    public async Task ReportsPageLoads()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");
        await Expect(Page).ToHaveTitleAsync(new Regex("Reports"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Reports" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ReportsPage_HasDateRangeInputs()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");
        await Expect(Page.Locator("#startDate")).ToBeVisibleAsync();
        await Expect(Page.Locator("#endDate")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ReportsPage_HasActionButtons()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");
        await Expect(Page.Locator("#export-pdf-btn")).ToBeVisibleAsync();
        await Expect(Page.Locator("#share-pdf-btn")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email Report")).ToBeVisibleAsync();
    }

    [Test]
    public async Task SettingsPageLoads()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");
        await Expect(Page).ToHaveTitleAsync(new Regex("Settings"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Settings" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("SMTP Email Settings")).ToBeVisibleAsync();
    }

    [Test]
    public async Task NavigationLinks_HighlightActiveSection()
    {
        // Dashboard
        await Page.GotoAsync(BaseUrl);
        await Expect(Page.Locator("nav a.active")).ToHaveTextAsync("Dashboard");

        // Members
        await Page.GotoAsync($"{BaseUrl}/members");
        await Expect(Page.Locator("nav a.active")).ToHaveTextAsync("Members");

        // Attendance
        await Page.GotoAsync($"{BaseUrl}/attendance");
        await Expect(Page.Locator("nav a.active")).ToHaveTextAsync("Attendance");

        // Reports
        await Page.GotoAsync($"{BaseUrl}/reports");
        await Expect(Page.Locator("nav a.active")).ToHaveTextAsync("Reports");

        // Settings
        await Page.GotoAsync($"{BaseUrl}/settings");
        await Expect(Page.Locator("nav a.active")).ToHaveTextAsync("Settings");
    }

    [Test]
    public async Task NavigationLinks_NavigateBetweenPages()
    {
        await Page.GotoAsync(BaseUrl);

        // Navigate to Members
        await Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Members" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("/members"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Members" })).ToBeVisibleAsync();

        // Navigate to Attendance
        await Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Attendance" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("/attendance"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Attendance" })).ToBeVisibleAsync();

        // Navigate to Reports
        await Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Reports" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("/reports"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Reports" })).ToBeVisibleAsync();

        // Navigate to Settings
        await Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Settings" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("/settings"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Settings" })).ToBeVisibleAsync();

        // Navigate back to Dashboard
        await Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Dashboard" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("/$"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })).ToBeVisibleAsync();
    }
}
