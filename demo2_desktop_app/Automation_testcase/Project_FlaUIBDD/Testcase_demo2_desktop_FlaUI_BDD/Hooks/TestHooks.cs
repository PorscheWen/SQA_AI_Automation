using System.Diagnostics;
using Demo2DesktopTests.Helpers;
using Demo2DesktopTests.PageObjects;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Demo2DesktopTests.Hooks;

[Binding]
public class TestHooks
{
    private static UIA3Automation? _automation;
    private static FlaUI.Core.Application? _application;
    private static Window? _mainWindow;
    private readonly ScenarioContext _scenarioContext;

    public TestHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        _automation = new UIA3Automation();
        var reportsDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "reports");
        HtmlReportHelper.InitializeReport(reportsDir, "Demo2TestReport.html");
    }

    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        HtmlReportHelper.CreateFeature(featureContext.FeatureInfo.Title, featureContext.FeatureInfo.Description ?? "");
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var title = _scenarioContext.ScenarioInfo.Title;
        HtmlReportHelper.CreateScenario(title, string.Join(", ", _scenarioContext.ScenarioInfo.Tags));
        if (_scenarioContext.ScenarioInfo.Tags.Length > 0)
        {
            HtmlReportHelper.AssignCategory(_scenarioContext.ScenarioInfo.Tags);
        }

        LaunchApplication();
        Thread.Sleep(2500);

        _mainWindow = WaitForMainWindow();
        if (_mainWindow == null)
        {
            throw new InvalidOperationException("無法取得 Demo2 主視窗");
        }

        _mainWindow.Focus();

        _scenarioContext.Set(_mainWindow, "MainWindow");
        _scenarioContext.Set(new MainWindowPage(_mainWindow, _automation!), "MainWindowPage");
        _scenarioContext.Set(new WorkspacePage(_mainWindow, _automation!), "WorkspacePage");
        _scenarioContext.Set(new FileDialogPage(_automation!), "FileDialogPage");
        _scenarioContext.Set(new MessageBoxPage(_automation!), "MessageBoxPage");
    }

    [AfterStep]
    public void AfterStep()
    {
        var stepInfo = _scenarioContext.StepContext?.StepInfo;
        if (stepInfo == null)
        {
            return;
        }

        var stepText = $"{stepInfo.StepDefinitionType} {stepInfo.Text}";
        if (_scenarioContext.TestError != null)
        {
            HtmlReportHelper.LogFail(stepText, _scenarioContext.TestError.Message, _scenarioContext.TestError.StackTrace ?? "");
        }
        else
        {
            HtmlReportHelper.LogPass(stepText);
        }
    }

    [AfterScenario]
    public void AfterScenario()
    {
        try
        {
            if (_scenarioContext.TestError != null && ConfigHelper.TakeScreenshotOnFailure())
            {
                TryCaptureFailureScreenshot();
            }
        }
        finally
        {
            CloseApplication();
        }
    }

    private void TryCaptureFailureScreenshot()
    {
        try
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, ConfigHelper.GetScreenshotDirectory());
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{Sanitize(_scenarioContext.ScenarioInfo.Title)}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            if (_mainWindow != null)
            {
                try
                {
                    ScreenshotHelper.TakeScreenshot(_mainWindow, path);
                    HtmlReportHelper.AttachScreenshot(path, "失敗截圖");
                    return;
                }
                catch
                {
                    // 對話框開啟時主視窗截圖常 UIA Timeout，改桌面截圖
                }
            }

            ScreenshotHelper.CaptureDesktop(path);
            HtmlReportHelper.AttachScreenshot(path, "失敗截圖（桌面）");
        }
        catch (Exception ex)
        {
            Console.WriteLine("截圖失敗: " + ex.Message);
        }
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        HtmlReportHelper.FlushReport();
        _automation?.Dispose();
    }

    public static void LaunchApplication()
    {
        var exe = ConfigHelper.GetApplicationPath();
        if (!File.Exists(exe))
        {
            throw new FileNotFoundException("Demo2Desktop.exe not found: " + exe);
        }

        var processName = ConfigHelper.GetProcessName();
        var existing = Process.GetProcessesByName(processName);
        if (existing.Length > 0)
        {
            _application = FlaUI.Core.Application.Attach(existing[0]);
            return;
        }

        _application = FlaUI.Core.Application.Launch(exe);
    }

    public static void CloseApplication()
    {
        DialogHelper.DismissOpenDialogs(_automation);

        try
        {
            _mainWindow?.Close();
        }
        catch
        {
            // ignore
        }

        try
        {
            _application?.Close();
        }
        catch
        {
            // ignore
        }

        _application?.Dispose();
        _application = null;
        _mainWindow = null;

        var processName = ConfigHelper.GetProcessName();
        foreach (var p in Process.GetProcessesByName(processName))
        {
            try
            {
                p.Kill();
                p.WaitForExit(3000);
            }
            catch
            {
                // ignore
            }
        }
    }

    public static void RelaunchApplication()
    {
        CloseApplication();
        Thread.Sleep(1000);
        LaunchApplication();
        Thread.Sleep(1500);
        _mainWindow = WaitForMainWindow();
        if (_mainWindow == null)
        {
            throw new InvalidOperationException("重新啟動後無法取得主視窗");
        }

        _mainWindow.Focus();
    }

    public static void BindPagesToScenario(ScenarioContext scenarioContext)
    {
        if (_mainWindow == null || _automation == null)
        {
            throw new InvalidOperationException("主視窗尚未就緒");
        }

        scenarioContext.Set(_mainWindow, "MainWindow");
        scenarioContext.Set(new MainWindowPage(_mainWindow, _automation), "MainWindowPage");
        scenarioContext.Set(new WorkspacePage(_mainWindow, _automation), "WorkspacePage");
        scenarioContext.Set(new FileDialogPage(_automation), "FileDialogPage");
        scenarioContext.Set(new MessageBoxPage(_automation), "MessageBoxPage");
    }

    private static Window? WaitForMainWindow()
    {
        var title = ConfigHelper.GetApplicationTitle();
        var timeout = TimeSpan.FromMilliseconds(ConfigHelper.GetDefaultTimeout());

        var perAttempt = TimeSpan.FromSeconds(2);
        return Retry.WhileNull(
            () =>
            {
                if (_application != null)
                {
                    try
                    {
                        var win = _application.GetMainWindow(_automation!, perAttempt);
                        if (win != null && win.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                        {
                            return win;
                        }
                    }
                    catch
                    {
                        // ignore single attempt timeout
                    }
                }

                try
                {
                    var desktop = _automation!.GetDesktop();
                    foreach (var w in desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window)))
                    {
                        var window = w.AsWindow();
                        if (window?.Title.Contains(title, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return window;
                        }
                    }
                }
                catch
                {
                    // ignore
                }

                return null;
            },
            timeout).Result;
    }

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}
