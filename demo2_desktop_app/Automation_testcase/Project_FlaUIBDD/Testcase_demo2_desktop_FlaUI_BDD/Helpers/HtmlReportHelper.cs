using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace Demo2DesktopTests.Helpers;

public static class HtmlReportHelper
{
    private static ExtentReports? _extent;
    private static ExtentTest? _feature;
    private static ExtentTest? _scenario;
    private static readonly object Lock = new();
    private static string _reportPath = string.Empty;

    public static void InitializeReport(string reportDirectory = "reports", string reportName = "Demo2TestReport.html")
    {
        lock (Lock)
        {
            if (_extent != null)
            {
                return;
            }

            Directory.CreateDirectory(reportDirectory);
            _reportPath = Path.Combine(reportDirectory, reportName);

            var htmlReporter = new ExtentSparkReporter(_reportPath);
            htmlReporter.Config.Theme = Theme.Standard;
            htmlReporter.Config.DocumentTitle = "FlaUI BDD 測試報告";
            htmlReporter.Config.ReportName = "Demo2 Desktop 自動化測試報告";
            htmlReporter.Config.Encoding = "UTF-8";

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("專案", "Demo2 Desktop");
            _extent.AddSystemInfo("框架", "FlaUI + SpecFlow + NUnit");
            _extent.AddSystemInfo("執行時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    public static void CreateFeature(string featureName, string description = "")
    {
        lock (Lock)
        {
            _extent ??= CreateExtentIfNeeded();
            _feature = _extent!.CreateTest(featureName, description);
        }
    }

    public static void CreateScenario(string scenarioName, string description = "")
    {
        lock (Lock)
        {
            if (_feature == null)
            {
                CreateFeature("Demo2 Desktop");
            }

            _scenario = _feature!.CreateNode(scenarioName, description);
        }
    }

    public static void LogPass(string stepName, string details = "")
    {
        lock (Lock)
        {
            _scenario?.Pass(stepName + (string.IsNullOrEmpty(details) ? "" : $": {details}"));
        }
    }

    public static void LogFail(string stepName, string errorMessage = "", string stackTrace = "")
    {
        lock (Lock)
        {
            var message = stepName;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                message += $"\n錯誤: {errorMessage}";
            }
            if (!string.IsNullOrEmpty(stackTrace))
            {
                message += $"\n{stackTrace}";
            }

            _scenario?.Fail(message);
        }
    }

    public static void AttachScreenshot(string screenshotPath, string title = "截圖")
    {
        lock (Lock)
        {
            if (_scenario != null && File.Exists(screenshotPath))
            {
                _scenario.AddScreenCaptureFromPath(screenshotPath, title);
            }
        }
    }

    public static void AssignCategory(params string[] categories)
    {
        lock (Lock)
        {
            if (_scenario == null)
            {
                return;
            }

            foreach (var category in categories)
            {
                _scenario.AssignCategory(category);
            }
        }
    }

    public static void FlushReport()
    {
        lock (Lock)
        {
            _extent?.Flush();
            _feature = null;
            _scenario = null;
        }
    }

    public static string GetReportPath() => _reportPath;

    private static ExtentReports CreateExtentIfNeeded()
    {
        InitializeReport();
        return _extent!;
    }
}
