using System.Diagnostics;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace ChurchAttendance.PlaywrightTests;

/// <summary>
/// Base class that starts the ChurchAttendance server before tests and stops it after.
/// All test fixtures should inherit from this instead of PageTest directly.
/// </summary>
public class PlaywrightTestBase : PageTest
{
    protected const string BaseUrl = "http://localhost:5050";

    private static Process? _serverProcess;
    private static readonly object _lock = new();
    private static int _fixtureCount;
    private static readonly TaskCompletionSource<bool> _serverReady = new();

    [OneTimeSetUp]
    public async Task BaseOneTimeSetUp()
    {
        bool isStarter = false;
        lock (_lock)
        {
            _fixtureCount++;
            if (_serverProcess is null)
            {
                isStarter = true;
                var projectDir = Path.GetFullPath(
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", "ChurchAttendance"));

                _serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = "run",
                        WorkingDirectory = projectDir,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Environment = { ["DOTNET_ENVIRONMENT"] = "Development" }
                    }
                };

                _serverProcess.Start();
            }
        }

        if (isStarter)
        {
            // Poll until server is ready, then signal all waiters
            using var httpClient = new HttpClient();
            for (var i = 0; i < 30; i++)
            {
                try
                {
                    var response = await httpClient.GetAsync(BaseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        _serverReady.TrySetResult(true);
                        return;
                    }
                }
                catch
                {
                    // Server not ready yet
                }
                await Task.Delay(1000);
            }

            _serverReady.TrySetException(new Exception("Server did not start within 30 seconds."));
        }

        // All fixtures wait for the server to be ready
        await _serverReady.Task;
    }

    [OneTimeTearDown]
    public void BaseOneTimeTearDown()
    {
        lock (_lock)
        {
            _fixtureCount--;
            if (_fixtureCount > 0 || _serverProcess is null)
                return;

            try
            {
                _serverProcess.Kill(entireProcessTree: true);
                _serverProcess.WaitForExit(5000);
            }
            catch
            {
                // Best effort cleanup
            }
            finally
            {
                _serverProcess.Dispose();
                _serverProcess = null;
            }
        }
    }
}
