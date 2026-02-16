using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ReportsTests : PlaywrightTestBase
{
    [Test]
    public async Task ReportsPage_DateRange_DefaultValues()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");

        var startDate = Page.Locator("#startDate");
        var endDate = Page.Locator("#endDate");

        // End date should default to today
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        await Expect(endDate).ToHaveValueAsync(today);

        // Start date should default to a week ago
        var weekAgo = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
        await Expect(startDate).ToHaveValueAsync(weekAgo);
    }

    [Test]
    public async Task ReportsPage_SetDateRange()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");

        var newStart = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
        var newEnd = DateTime.Today.ToString("yyyy-MM-dd");

        await Page.Locator("#startDate").FillAsync(newStart);
        await Page.Locator("#endDate").FillAsync(newEnd);

        await Expect(Page.Locator("#startDate")).ToHaveValueAsync(newStart);
        await Expect(Page.Locator("#endDate")).ToHaveValueAsync(newEnd);
    }

    [Test]
    public async Task ReportsPage_DownloadPdf_ReturnsPdfResponse()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");

        // Set a date range
        var startDate = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
        var endDate = DateTime.Today.ToString("yyyy-MM-dd");
        await Page.Locator("#startDate").FillAsync(startDate);
        await Page.Locator("#endDate").FillAsync(endDate);

        // Wait for the hidden form fields to sync
        await Page.WaitForTimeoutAsync(500);

        // Click "Download PDF" and intercept the download
        var downloadTask = Page.WaitForDownloadAsync();
        await Page.Locator("#export-pdf-btn").ClickAsync();
        var download = await downloadTask;

        // Verify the file name pattern
        Assert.That(download.SuggestedFilename, Does.Contain("Church-Attendance-Report"));
        Assert.That(download.SuggestedFilename, Does.EndWith(".pdf"));
    }

    [Test]
    public async Task ReportsPage_DateInputs_SyncWithHiddenFields()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");

        var newStart = "2025-01-01";
        var newEnd = "2025-01-31";

        await Page.Locator("#startDate").FillAsync(newStart);
        // Trigger input event for sync
        await Page.Locator("#startDate").DispatchEventAsync("input");

        await Page.Locator("#endDate").FillAsync(newEnd);
        await Page.Locator("#endDate").DispatchEventAsync("input");

        // Hidden form fields should be synced
        var hiddenStartValues = await Page.Locator(".report-start-date").EvaluateAllAsync<string[]>(
            "elements => elements.map(e => e.value)");
        var hiddenEndValues = await Page.Locator(".report-end-date").EvaluateAllAsync<string[]>(
            "elements => elements.map(e => e.value)");

        foreach (var val in hiddenStartValues)
            Assert.That(val, Is.EqualTo(newStart));
        foreach (var val in hiddenEndValues)
            Assert.That(val, Is.EqualTo(newEnd));
    }

    [Test]
    public async Task ReportsPage_HasAllActionButtons()
    {
        await Page.GotoAsync($"{BaseUrl}/reports");

        await Expect(Page.Locator("#export-pdf-btn")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Download PDF")).ToBeVisibleAsync();
        await Expect(Page.Locator("#share-pdf-btn")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Share PDF")).ToBeVisibleAsync();
    }
}
