using System;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using TechTalk.SpecFlow;
using ShoppingCartTests.PageObjects;
using ShoppingCartTests.Helpers;
using NUnit.Framework;

namespace ShoppingCartTests.Hooks
{
    [Binding]
    public class TestHooks
    {
        private static UIA3Automation _automation;
        private static FlaUI.Core.Application _application;
        private static Window _mainWindow;
        private static bool _launchedWebViewHost;
        private readonly ScenarioContext _scenarioContext;

        public TestHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        /// <summary>
        /// 測試執行前的設定（所有測試執行前只執行一次）
        /// </summary>
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Console.WriteLine("=== 測試開始 ===");
            _automation = new UIA3Automation();
        }

        [BeforeStep]
        public void BeforeStep()
        {
            var stepInfo = _scenarioContext.StepContext?.StepInfo;
            if (stepInfo != null && !string.IsNullOrWhiteSpace(stepInfo.Text))
            {
                _scenarioContext["CurrentStep"] = $"{stepInfo.StepDefinitionType} {stepInfo.Text}";
            }
        }

        /// <summary>
        /// 每個 Scenario 執行前的設定
        /// </summary>
        [BeforeScenario]
        public void BeforeScenario()
        {
            Console.WriteLine($"=== 開始執行場景: {_scenarioContext.ScenarioInfo.Title} ===");

            try
            {
                LaunchApplication();
                System.Threading.Thread.Sleep(2000);

                _mainWindow = WaitForBrowserWindow();
                if (_mainWindow == null)
                {
                    throw new Exception("無法取得瀏覽器主視窗");
                }

                WaitForPageReady();
                System.Threading.Thread.Sleep(1000);

                // 建立 Page Object 並存入 ScenarioContext
                var productListPage = new ProductListPage(_mainWindow, _automation);
                var shoppingCartPage = new ShoppingCartPage(_mainWindow, _automation);

                _scenarioContext.Add("ProductListPage", productListPage);
                _scenarioContext.Add("ShoppingCartPage", shoppingCartPage);
                _scenarioContext.Add("MainWindow", _mainWindow);

                Console.WriteLine("測試環境初始化完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試初始化失敗: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 每個 Scenario 執行後的清理
        /// </summary>
        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                // 如果測試失敗，截圖
                if (_scenarioContext.TestError != null)
                {
                    Console.WriteLine($"測試失敗: {_scenarioContext.TestError.Message}");
                    var screenshotPath = TakeScreenshot();

                    if (ConfigHelper.WriteFailureLogOnFailure())
                    {
                        FailureLogHelper.WriteFailureLog(_scenarioContext, _mainWindow, screenshotPath);
                    }
                }

                // 清理 Page Objects
                if (_scenarioContext.ContainsKey("ProductListPage"))
                {
                    var page = _scenarioContext.Get<ProductListPage>("ProductListPage");
                    page?.Dispose();
                }

                if (_scenarioContext.ContainsKey("ShoppingCartPage"))
                {
                    var page = _scenarioContext.Get<ShoppingCartPage>("ShoppingCartPage");
                    page?.Dispose();
                }

                // 關閉應用程式
                _mainWindow?.Close();
                _application?.Close();
                _application?.Dispose();

                Console.WriteLine($"=== 場景執行完畢: {_scenarioContext.ScenarioInfo.Title} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試清理失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試執行後的清理（所有測試執行後只執行一次）
        /// </summary>
        [AfterTestRun]
        public static void AfterTestRun()
        {
            _automation?.Dispose();
            Console.WriteLine("=== 測試結束 ===");
        }

        /// <summary>
        /// 截圖
        /// </summary>
        private string TakeScreenshot()
        {
            try
            {
                if (!ConfigHelper.TakeScreenshotOnFailure())
                {
                    return string.Empty;
                }

                var screenshotDir = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    ConfigHelper.GetScreenshotDirectory()
                );
                Directory.CreateDirectory(screenshotDir);

                var fileName = $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(screenshotDir, fileName);

                if (_mainWindow != null)
                {
                    ScreenshotHelper.TakeScreenshot(_mainWindow, filePath);
                    TestContext.AddTestAttachment(filePath, "失敗截圖");
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"截圖失敗: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// 啟動 WebView2 宿主或瀏覽器開啟測試網頁
        /// </summary>
        private static void LaunchApplication()
        {
            var appUrl = ConfigHelper.GetApplicationUrl();
            var appPath = ConfigHelper.GetApplicationPath();

            if (appPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(appPath))
            {
                _launchedWebViewHost = true;
                _application = FlaUI.Core.Application.Launch(appPath, appUrl);
                Console.WriteLine($"WebView 宿主已啟動: {appPath} -> {appUrl}");
                return;
            }

            _launchedWebViewHost = false;
            LaunchBrowserApplication(appUrl);
        }

        /// <summary>
        /// 以瀏覽器開啟測試網頁（需先啟動 Python demo server）
        /// </summary>
        private static void LaunchBrowserApplication(string appUrl)
        {
            var browserPath = ResolveBrowserPath(ConfigHelper.GetBrowserType());
            var isChrome = browserPath.Contains("chrome", StringComparison.OrdinalIgnoreCase);
            var isEdge = browserPath.Contains("msedge", StringComparison.OrdinalIgnoreCase) ||
                         browserPath.Contains("edge", StringComparison.OrdinalIgnoreCase);
            var userDataDir = Path.Combine(Path.GetTempPath(), "FlaUIBDD_BrowserProfile");
            Directory.CreateDirectory(userDataDir);

            var arguments = isChrome
                ? $"--user-data-dir=\"{userDataDir}\" --force-renderer-accessibility --no-first-run --disable-default-apps --new-window \"{appUrl}\""
                : isEdge
                    ? $"--user-data-dir=\"{userDataDir}\" --force-renderer-accessibility --inprivate --new-window \"{appUrl}\""
                    : $"--new-window \"{appUrl}\"";

            _application = FlaUI.Core.Application.Launch(browserPath, arguments);
            Console.WriteLine($"瀏覽器已啟動: {browserPath} -> {appUrl}");
        }

        private static void WaitForPageReady()
        {
            Retry.WhileTrue(
                () =>
                {
                    if (_launchedWebViewHost && _application != null)
                    {
                        try
                        {
                            var hostWindow = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(1));
                            return hostWindow?.FindFirstDescendant(cf => cf.ByAutomationId("btn-add-apple")) == null;
                        }
                        catch
                        {
                            return true;
                        }
                    }

                    return FindShoppingPageRoot()?.FindFirstDescendant(cf => cf.ByAutomationId("btn-add-apple")) == null;
                },
                TimeSpan.FromSeconds(30)
            );
        }

        private static AutomationElement FindShoppingPageRoot()
        {
            var desktop = _automation.GetDesktop();
            var addButton = desktop.FindFirstDescendant(cf => cf.ByAutomationId("btn-add-apple"));
            if (addButton != null)
            {
                return addButton;
            }

            return _mainWindow?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Document)) ?? _mainWindow;
        }

        /// <summary>
        /// 等待瀏覽器視窗出現
        /// </summary>
        private static Window WaitForBrowserWindow()
        {
            return Retry.WhileNull(
                () => FindBrowserWindow(),
                TimeSpan.FromSeconds(15)
            ).Result;
        }

        /// <summary>
        /// 依標題尋找購物車 demo 頁面視窗
        /// </summary>
        private static Window FindBrowserWindow()
        {
            if (_launchedWebViewHost && _application != null)
            {
                try
                {
                    var hostWindow = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
                    if (hostWindow != null &&
                        !hostWindow.Title.Contains("Edge", StringComparison.OrdinalIgnoreCase) &&
                        !hostWindow.Title.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                    {
                        return hostWindow;
                    }

                    foreach (var window in _application.GetAllTopLevelWindows(_automation))
                    {
                        if (!window.Title.Contains("Edge", StringComparison.OrdinalIgnoreCase) &&
                            !window.Title.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                        {
                            return window;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"無法從 WebView 宿主取得視窗: {ex.Message}");
                }

                return null;
            }

            var desktop = _automation.GetDesktop();
            var addButton = desktop.FindFirstDescendant(cf => cf.ByAutomationId("btn-add-apple"));
            if (addButton != null)
            {
                var parentWindow = addButton.Parent;
                while (parentWindow != null && parentWindow.ControlType != ControlType.Window)
                {
                    parentWindow = parentWindow.Parent;
                }

                var hostWindow = parentWindow?.AsWindow();
                if (hostWindow != null)
                {
                    return hostWindow;
                }
            }

            var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

            foreach (var element in windows)
            {
                var window = element.AsWindow();
                if (window == null || string.IsNullOrEmpty(window.Title))
                {
                    continue;
                }

                if (window.Title.Contains("Demo Shop", StringComparison.OrdinalIgnoreCase) ||
                    window.Title.Contains("購物車", StringComparison.OrdinalIgnoreCase))
                {
                    if (!window.Title.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) &&
                        !window.Title.Contains("Edge", StringComparison.OrdinalIgnoreCase) &&
                        !window.Title.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                    {
                        return window;
                    }
                }
            }

            foreach (var element in windows)
            {
                var window = element.AsWindow();
                if (window == null || string.IsNullOrEmpty(window.Title))
                {
                    continue;
                }

                if (window.Title.Contains("Demo Shop", StringComparison.OrdinalIgnoreCase) ||
                    window.Title.Contains("購物車", StringComparison.OrdinalIgnoreCase) ||
                    window.Title.Contains("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    return window;
                }
            }

            if (_application != null)
            {
                try
                {
                    return _application.GetMainWindow(_automation, TimeSpan.FromSeconds(3));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"無法從程序取得主視窗: {ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// 解析瀏覽器執行檔路徑
        /// </summary>
        private static string ResolveBrowserPath(string browserType)
        {
            var candidates = browserType.Equals("Edge", StringComparison.OrdinalIgnoreCase) ||
                             browserType.Equals("MsEdge", StringComparison.OrdinalIgnoreCase)
                ? new[]
                {
                    @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                    @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
                    "msedge"
                }
                : new[]
                {
                    @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    "chrome"
                };

            foreach (var candidate in candidates)
            {
                if (candidate.Contains('\\', StringComparison.Ordinal) && File.Exists(candidate))
                {
                    return candidate;
                }

                if (!candidate.Contains('\\', StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException(
                $"找不到 {browserType} 瀏覽器，請安裝 Chrome 或將 App.config 的 BrowserType 改為 Edge。");
        }
    }
}
