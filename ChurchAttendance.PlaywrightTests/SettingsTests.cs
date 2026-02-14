using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SettingsTests : PlaywrightTestBase
{
    [Test]
    public async Task SettingsPage_HasSmtpForm()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");

        await Expect(Page.Locator("#host")).ToBeVisibleAsync();
        await Expect(Page.Locator("#port")).ToBeVisibleAsync();
        await Expect(Page.Locator("#username")).ToBeVisibleAsync();
        await Expect(Page.Locator("#password")).ToBeVisibleAsync();
        await Expect(Page.Locator("#fromEmail")).ToBeVisibleAsync();
        await Expect(Page.Locator("#toEmail")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='useSsl']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task SettingsPage_DefaultPort_Is587()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");
        await Expect(Page.Locator("#port")).ToHaveValueAsync("587");
    }

    [Test]
    public async Task SettingsPage_SaveSmtpSettings_ShowsSuccess()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");

        // Fill in SMTP settings
        await Page.Locator("#host").FillAsync("smtp.test.example.com");
        await Page.Locator("#port").FillAsync("465");
        await Page.Locator("#username").FillAsync("testuser@example.com");
        await Page.Locator("#password").FillAsync("testpassword123");
        await Page.Locator("#fromEmail").FillAsync("from@example.com");
        await Page.Locator("#toEmail").FillAsync("to@example.com");

        // Check SSL
        await Page.Locator("input[name='useSsl']").CheckAsync();

        // Submit the form
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save Settings" }).ClickAsync();

        // Should show success message
        var status = Page.Locator("#settings-status");
        await Expect(status).ToContainTextAsync("SMTP settings saved!", new() { Timeout = 5000 });
    }

    [Test]
    public async Task SettingsPage_SaveAndReload_PersistedValues()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");

        var uniqueHost = $"smtp-{Guid.NewGuid():N}"[..20] + ".example.com";

        await Page.Locator("#host").FillAsync(uniqueHost);
        await Page.Locator("#port").FillAsync("587");
        await Page.Locator("#username").FillAsync("persist@example.com");
        await Page.Locator("#password").FillAsync("persistpass");
        await Page.Locator("#fromEmail").FillAsync("persist-from@example.com");
        await Page.Locator("#toEmail").FillAsync("persist-to@example.com");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save Settings" }).ClickAsync();
        await Expect(Page.Locator("#settings-status")).ToContainTextAsync("SMTP settings saved!");

        // Reload the page
        await Page.ReloadAsync();

        // Verify values persisted
        await Expect(Page.Locator("#host")).ToHaveValueAsync(uniqueHost);
        await Expect(Page.Locator("#username")).ToHaveValueAsync("persist@example.com");
        await Expect(Page.Locator("#fromEmail")).ToHaveValueAsync("persist-from@example.com");
        await Expect(Page.Locator("#toEmail")).ToHaveValueAsync("persist-to@example.com");
    }

    [Test]
    public async Task SettingsPage_HasSaveButton()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Save Settings" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task SettingsPage_SslCheckbox_Toggles()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");

        var sslCheckbox = Page.Locator("input[name='useSsl']");

        // Check it
        await sslCheckbox.CheckAsync();
        await Expect(sslCheckbox).ToBeCheckedAsync();

        // Uncheck it
        await sslCheckbox.UncheckAsync();
        await Expect(sslCheckbox).Not.ToBeCheckedAsync();
    }
}
