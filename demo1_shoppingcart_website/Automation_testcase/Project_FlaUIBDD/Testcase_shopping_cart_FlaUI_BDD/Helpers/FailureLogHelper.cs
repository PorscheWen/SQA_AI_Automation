using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace ShoppingCartTests.Helpers
{
    /// <summary>
    /// 測試失敗時產生診斷 log，協助修正問題
    /// </summary>
    public static class FailureLogHelper
    {
        public static string WriteFailureLog(
            ScenarioContext scenarioContext,
            Window mainWindow,
            string screenshotPath)
        {
            var logDirectory = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                ConfigHelper.GetFailureLogDirectory());
            Directory.CreateDirectory(logDirectory);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeTitle = SanitizeFileName(scenarioContext.ScenarioInfo.Title);
            var logPath = Path.Combine(logDirectory, $"{safeTitle}_{timestamp}_failure.log");

            var content = BuildLogContent(scenarioContext, mainWindow, screenshotPath);
            File.WriteAllText(logPath, content, Encoding.UTF8);

            Console.WriteLine($"失敗 log 已儲存: {logPath}");
            TestContext.AddTestAttachment(logPath, "失敗診斷 log");

            return logPath;
        }

        private static string BuildLogContent(
            ScenarioContext scenarioContext,
            Window mainWindow,
            string screenshotPath)
        {
            var error = scenarioContext.TestError;
            var builder = new StringBuilder();

            builder.AppendLine("========================================");
            builder.AppendLine("  FlaUI BDD 測試失敗診斷報告");
            builder.AppendLine("========================================");
            builder.AppendLine($"時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine($"場景: {scenarioContext.ScenarioInfo.Title}");
            builder.AppendLine($"標籤: {FormatTags(scenarioContext)}");
            builder.AppendLine($"失敗步驟: {GetFailedStepText(scenarioContext)}");
            builder.AppendLine();

            builder.AppendLine("[錯誤摘要]");
            builder.AppendLine($"類型: {error?.GetType().FullName ?? "Unknown"}");
            builder.AppendLine($"訊息: {error?.Message ?? "(無)"}");
            builder.AppendLine();

            if (error?.InnerException != null)
            {
                builder.AppendLine("[內部例外]");
                builder.AppendLine($"類型: {error.InnerException.GetType().FullName}");
                builder.AppendLine($"訊息: {error.InnerException.Message}");
                builder.AppendLine();
            }

            builder.AppendLine("[堆疊追蹤]");
            builder.AppendLine(error?.StackTrace ?? "(無)");
            builder.AppendLine();

            builder.AppendLine("[執行環境]");
            builder.AppendLine($"ApplicationUrl: {ConfigHelper.GetApplicationUrl()}");
            builder.AppendLine($"ApplicationPath: {ConfigHelper.GetApplicationPath()}");
            builder.AppendLine($"BrowserType: {ConfigHelper.GetBrowserType()}");
            builder.AppendLine($"DefaultTimeout(ms): {ConfigHelper.GetDefaultTimeout()}");
            builder.AppendLine($"工作目錄: {TestContext.CurrentContext.WorkDirectory}");
            builder.AppendLine($"視窗標題: {mainWindow?.Title ?? "(無)"}");
            builder.AppendLine($"使用 WebView 宿主: {ConfigHelper.GetApplicationPath().EndsWith(".exe", StringComparison.OrdinalIgnoreCase)}");
            builder.AppendLine();

            builder.AppendLine("[UI 狀態快照]");
            AppendUiSnapshot(builder, scenarioContext, mainWindow);
            builder.AppendLine();

            builder.AppendLine("[附件]");
            builder.AppendLine($"截圖: {(string.IsNullOrWhiteSpace(screenshotPath) ? "(未產生)" : screenshotPath)}");
            builder.AppendLine();

            builder.AppendLine("[修正建議]");
            foreach (var suggestion in GetCorrectionSuggestions(error, scenarioContext, mainWindow))
            {
                builder.AppendLine($"- {suggestion}");
            }
            builder.AppendLine();

            builder.AppendLine("[快速檢查清單]");
            builder.AppendLine("1. 確認 demo server 已啟動: cd demo\\shopping_cart && python serve.py");
            builder.AppendLine("2. 確認 WebView 宿主已建置: dotnet build -c Release (ShoppingCartWebViewHost)");
            builder.AppendLine("3. 手動開啟: http://localhost:8888/");
            builder.AppendLine("4. 檢查 App.config 的 ApplicationPath / ApplicationUrl");
            builder.AppendLine("5. 對照 Screenshots 與 FailureLogs 內容排查");

            return builder.ToString();
        }

        private static void AppendUiSnapshot(
            StringBuilder builder,
            ScenarioContext scenarioContext,
            Window mainWindow)
        {
            if (mainWindow == null)
            {
                builder.AppendLine("主視窗: 未取得");
                return;
            }

            builder.AppendLine("主視窗: 已取得");

            if (scenarioContext.ContainsKey("ShoppingCartPage"))
            {
                try
                {
                    var cartPage = scenarioContext.Get<PageObjects.ShoppingCartPage>("ShoppingCartPage");
                    builder.AppendLine($"購物車件數(實際): {SafeRead(() => cartPage.GetCartCount())}");
                    builder.AppendLine($"購物車總計(實際): {SafeRead(() => cartPage.GetCartTotal())}");
                }
                catch (Exception ex)
                {
                    builder.AppendLine($"讀取購物車狀態失敗: {ex.Message}");
                }
            }
            else
            {
                builder.AppendLine("ShoppingCartPage: 尚未初始化");
            }

            try
            {
                var addButtonExists = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("btn-add-apple")) != null;
                builder.AppendLine($"btn-add-apple 可見: {addButtonExists}");
            }
            catch (Exception ex)
            {
                builder.AppendLine($"btn-add-apple 檢查失敗: {ex.Message}");
            }
        }

        private static IEnumerable<string> GetCorrectionSuggestions(
            Exception? error,
            ScenarioContext scenarioContext,
            Window? mainWindow)
        {
            var suggestions = new List<string>();
            var message = error?.Message ?? string.Empty;
            var windowTitle = mainWindow?.Title ?? string.Empty;

            if (error is ElementNotAvailableException || message.Contains("AutomationId", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("元素定位失敗：確認 WebView 宿主已啟動且頁面已載入完成（非空白頁）。");
                suggestions.Add("確認 python serve.py 正在 port 8888 提供 demo 頁面。");
                suggestions.Add("HTML 元素需使用 id 屬性（如 btn-add-apple），WebView2 才會映射為 AutomationId。");
            }

            if (message.Contains("not a valid application", StringComparison.OrdinalIgnoreCase) ||
                message.Contains(".html", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("不可直接用 Application.Launch 開啟 .html；請使用 ShoppingCartWebViewHost.exe。");
                suggestions.Add("更新 App.config 的 ApplicationPath 指向 WebView 宿主 exe。");
            }

            if (message.Contains("Ambiguous step definitions", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("SpecFlow 步驟定義重複：檢查 StepDefinitions 是否有相同 Gherkin 步驟。");
            }

            if (message.Contains("購物車件數不符", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("購物車總計不符", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Assert", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("斷言失敗：比對 log 中「UI 狀態快照」與 Feature 預期值。");
                suggestions.Add("若實際值未更新，可能是點擊未生效或連到錯誤視窗（例如 Edge 分頁而非 WebView 宿主）。");
                suggestions.Add("關閉其他已開啟的 Demo Shop 瀏覽器分頁，僅保留 WebView 宿主視窗。");
            }

            if (windowTitle.Contains("Edge", StringComparison.OrdinalIgnoreCase) ||
                windowTitle.Contains("Chrome", StringComparison.OrdinalIgnoreCase) ||
                windowTitle.Contains("Microsoft", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("偵測到瀏覽器視窗標題：測試可能誤連 Edge/Chrome。請確認 ApplicationPath 指向 ShoppingCartWebViewHost.exe，並關閉其他 Demo Shop 分頁。");
            }

            if (message.Contains("Could not find process", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("無法取得", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("應用程式啟動或視窗取得失敗：先建置 WebView 宿主，並關閉殘留的 ShoppingCartWebViewHost 程序。");
            }

            if (message.Contains("localhost:8080", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add("URL 設定錯誤：App.config 應為 http://localhost:8888。");
            }

            if (suggestions.Count == 0)
            {
                suggestions.Add("請先查看截圖與堆疊追蹤，確認失敗發生在哪個步驟。");
                suggestions.Add($"失敗步驟: {GetFailedStepText(scenarioContext)}");
            }

            return suggestions.Distinct();
        }

        private static string GetFailedStepText(ScenarioContext scenarioContext)
        {
            if (scenarioContext.ContainsKey("CurrentStep"))
            {
                return scenarioContext.Get<string>("CurrentStep");
            }

            var stepInfo = scenarioContext.StepContext?.StepInfo;
            if (stepInfo != null && !string.IsNullOrWhiteSpace(stepInfo.Text))
            {
                return $"{stepInfo.StepDefinitionType} {stepInfo.Text}";
            }

            return "(無法判定)";
        }

        private static string FormatTags(ScenarioContext scenarioContext)
        {
            var tags = scenarioContext.ScenarioInfo.Tags?.ToList() ?? new List<string>();
            return tags.Count == 0 ? "(無)" : string.Join(", ", tags);
        }

        private static string SafeRead(Func<string> readValue)
        {
            try
            {
                return readValue() ?? "(空)";
            }
            catch (Exception ex)
            {
                return $"(讀取失敗: {ex.Message})";
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }
    }
}
