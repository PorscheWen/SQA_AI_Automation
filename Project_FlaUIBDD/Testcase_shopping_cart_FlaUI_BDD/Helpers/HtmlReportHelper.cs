using System;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;

namespace ShoppingCartTests.Helpers
{
    /// <summary>
    /// HTML 測試報告生成器
    /// 使用 ExtentReports 生成美觀的 HTML 測試報告
    /// </summary>
    public class HtmlReportHelper
    {
        private static ExtentReports? _extent;
        private static ExtentTest? _feature;
        private static ExtentTest? _scenario;
        private static readonly object _lock = new object();
        private static string _reportPath = string.Empty;

        /// <summary>
        /// 初始化報告（在測試開始前呼叫）
        /// </summary>
        public static void InitializeReport(string reportDirectory = "reports", string reportName = "TestReport.html")
        {
            lock (_lock)
            {
                if (_extent != null)
                {
                    return; // 已經初始化過了
                }

                // 建立報告目錄
                Directory.CreateDirectory(reportDirectory);
                _reportPath = Path.Combine(reportDirectory, reportName);

                // 建立 HTML Reporter
                var htmlReporter = new ExtentSparkReporter(_reportPath);

                // 配置報告樣式
                htmlReporter.Config.Theme = Theme.Standard;
                htmlReporter.Config.DocumentTitle = "FlaUI BDD 測試報告";
                htmlReporter.Config.ReportName = "Shopping Cart 自動化測試報告";
                htmlReporter.Config.Encoding = "UTF-8";
                htmlReporter.Config.TimeStampFormat = "yyyy-MM-dd HH:mm:ss";

                // 初始化 ExtentReports
                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);

                // 添加系統資訊
                _extent.AddSystemInfo("測試環境", "Windows");
                _extent.AddSystemInfo("自動化框架", "FlaUI + SpecFlow");
                _extent.AddSystemInfo("測試類型", "UI 自動化測試");
                _extent.AddSystemInfo("專案", "Shopping Cart");
                _extent.AddSystemInfo("測試執行時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                Console.WriteLine($"HTML 測試報告已初始化: {_reportPath}");
            }
        }

        /// <summary>
        /// 建立 Feature
        /// </summary>
        public static void CreateFeature(string featureName, string description = "")
        {
            lock (_lock)
            {
                if (_extent == null)
                {
                    InitializeReport();
                }

                _feature = _extent.CreateTest(featureName, description);
                Console.WriteLine($"建立 Feature: {featureName}");
            }
        }

        /// <summary>
        /// 建立 Scenario
        /// </summary>
        public static void CreateScenario(string scenarioName, string description = "")
        {
            lock (_lock)
            {
                if (_feature == null)
                {
                    CreateFeature("Default Feature");
                }

                _scenario = _feature.CreateNode(scenarioName, description);
                Console.WriteLine($"建立 Scenario: {scenarioName}");
            }
        }

        /// <summary>
        /// 記錄測試步驟 - 通過
        /// </summary>
        public static void LogPass(string stepName, string details = "")
        {
            lock (_lock)
            {
                if (_scenario == null)
                {
                    Console.WriteLine("警告: Scenario 尚未建立，無法記錄步驟");
                    return;
                }

                _scenario.Pass(stepName + (string.IsNullOrEmpty(details) ? "" : $": {details}"));
            }
        }

        /// <summary>
        /// 記錄測試步驟 - 失敗
        /// </summary>
        public static void LogFail(string stepName, string errorMessage = "", string stackTrace = "")
        {
            lock (_lock)
            {
                if (_scenario == null)
                {
                    Console.WriteLine("警告: Scenario 尚未建立，無法記錄步驟");
                    return;
                }

                var message = stepName;
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    message += $"\n錯誤訊息: {errorMessage}";
                }
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    message += $"\n堆疊追蹤:\n{stackTrace}";
                }

                _scenario.Fail(message);
            }
        }

        /// <summary>
        /// 記錄測試步驟 - 跳過
        /// </summary>
        public static void LogSkip(string stepName, string reason = "")
        {
            lock (_lock)
            {
                if (_scenario == null)
                {
                    Console.WriteLine("警告: Scenario 尚未建立，無法記錄步驟");
                    return;
                }

                _scenario.Skip(stepName + (string.IsNullOrEmpty(reason) ? "" : $": {reason}"));
            }
        }

        /// <summary>
        /// 記錄測試步驟 - 資訊
        /// </summary>
        public static void LogInfo(string message)
        {
            lock (_lock)
            {
                if (_scenario == null)
                {
                    Console.WriteLine("警告: Scenario 尚未建立，無法記錄資訊");
                    return;
                }

                _scenario.Info(message);
            }
        }

        /// <summary>
        /// 記錄測試步驟 - 警告
        /// </summary>
        public static void LogWarning(string message)
        {
            lock (_lock)
            {
                if (_scenario == null)
                {
                    Console.WriteLine("警告: Scenario 尚未建立，無法記錄警告");
                    return;
                }

                _scenario.Warning(message);
            }
        }

        /// <summary>
        /// 添加截圖到報告
        /// </summary>
        public static void AttachScreenshot(string screenshotPath, string title = "截圖")
        {
            lock (_lock)
            {
                if (_scenario == null || string.IsNullOrEmpty(screenshotPath))
                {
                    return;
                }

                if (File.Exists(screenshotPath))
                {
                    _scenario.AddScreenCaptureFromPath(screenshotPath, title);
                    Console.WriteLine($"已添加截圖到報告: {screenshotPath}");
                }
            }
        }

        /// <summary>
        /// 設定 Scenario 標籤
        /// </summary>
        public static void AssignCategory(params string[] categories)
        {
            lock (_lock)
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

        /// <summary>
        /// 結束報告並儲存（在所有測試完成後呼叫）
        /// </summary>
        public static void FlushReport()
        {
            lock (_lock)
            {
                if (_extent == null)
                {
                    return;
                }

                _extent.Flush();
                Console.WriteLine($"HTML 測試報告已生成: {_reportPath}");

                // 清空靜態變數，避免下次執行時重複使用
                _feature = null;
                _scenario = null;
            }
        }

        /// <summary>
        /// 取得報告路徑
        /// </summary>
        public static string GetReportPath()
        {
            return _reportPath;
        }
    }
}
