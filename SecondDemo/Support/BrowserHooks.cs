using Microsoft.Playwright;
using Reqnroll;

namespace SecondDemo.Support;

[Binding]
public sealed class BrowserHooks
{
    private static readonly string TestResultsDirectory = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestResults"));

    private readonly ScenarioContext _scenarioContext;

    public BrowserHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public async Task StartBrowser()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
        var scenarioName = ToSafeFileName(_scenarioContext.ScenarioInfo.Title);
        var screenshotDirectory = Path.Combine(TestResultsDirectory, $"{timestamp}_{scenarioName}");
        Directory.CreateDirectory(screenshotDirectory);
        _scenarioContext["ScreenshotDirectory"] = screenshotDirectory;
        _scenarioContext["ScreenshotIndex"] = 0;

        var playwright = await Playwright.CreateAsync();
        var headless = bool.TryParse(Environment.GetEnvironmentVariable("PLAYWRIGHT_HEADLESS"), out var configuredHeadless)
            && configuredHeadless;
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = headless
        };
        var executablePath = Environment.GetEnvironmentVariable("PLAYWRIGHT_CHROMIUM_EXECUTABLE");
        if (!string.IsNullOrWhiteSpace(executablePath))
        {
            launchOptions.ExecutablePath = executablePath;
        }

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var page = await browser.NewPageAsync();

        _scenarioContext["Playwright"] = playwright;
        _scenarioContext["Browser"] = browser;
        _scenarioContext["Page"] = page;
    }

    [AfterStep]
    public async Task CaptureStepScreenshot()
    {
        if (!_scenarioContext.TryGetValue("Page", out var pageValue) || pageValue is not IPage page || page.IsClosed)
        {
            return;
        }

        if (!_scenarioContext.TryGetValue("ScreenshotDirectory", out var directoryValue)
            || directoryValue is not string screenshotDirectory)
        {
            return;
        }

        var screenshotIndex = _scenarioContext.TryGetValue("ScreenshotIndex", out var indexValue)
            && indexValue is int currentIndex
                ? currentIndex + 1
                : 1;
        _scenarioContext["ScreenshotIndex"] = screenshotIndex;
        var stepName = ToSafeFileName(_scenarioContext.StepContext.StepInfo.Text);
        var screenshotPath = Path.Combine(screenshotDirectory, $"{screenshotIndex:D2}_{stepName}.png");

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = screenshotPath,
            FullPage = true
        });
    }

    [AfterScenario]
    public async Task StopBrowser()
    {
        if (_scenarioContext.TryGetValue("Browser", out var browserValue) && browserValue is IBrowser browser)
        {
            await browser.CloseAsync();
        }

        if (_scenarioContext.TryGetValue("Playwright", out var playwrightValue) && playwrightValue is IPlaywright playwright)
        {
            playwright.Dispose();
        }
    }

    private static string ToSafeFileName(string value)
    {
        var safeName = string.Concat(value.Select(character => char.IsLetterOrDigit(character) ? character : '_'));
        return safeName.Length <= 80 ? safeName : safeName[..80];
    }
}