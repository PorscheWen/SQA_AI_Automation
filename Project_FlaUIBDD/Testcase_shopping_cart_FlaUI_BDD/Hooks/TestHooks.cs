using System;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
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

        /// <summary>
        /// 每個 Scenario 執行前的設定
        /// </summary>
        [BeforeScenario]
        public void BeforeScenario()
        {
            Console.WriteLine($"=== 開始執行場景: {_scenarioContext.ScenarioInfo.Title} ===");

            try
            {
                // 啟動應用程式
                // 注意: 請根據實際應用程式路徑修改
                string appPath = GetApplicationPath();

                if (File.Exists(appPath))
                {
                    _application = FlaUI.Core.Application.Launch(appPath);
                    Console.WriteLine($"應用程式已啟動: {appPath}");
                }
                else
                {
                    // 如果是 Web 應用，使用瀏覽器啟動
                    // 例如: Chrome, Edge 等
                    Console.WriteLine("使用瀏覽器啟動應用程式");
                    // _application = Application.Launch("chrome.exe", "http://localhost:8080");
                }

                // 等待主視窗出現
                _mainWindow = _application?.GetMainWindow(_automation, TimeSpan.FromSeconds(10));

                if (_mainWindow == null)
                {
                    throw new Exception("無法取得主視窗");
                }

                // Wait for window to be responsive
                // Wait.UntilResponsive(_mainWindow, TimeSpan.FromSeconds(5));
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
                    TakeScreenshot();
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
        private void TakeScreenshot()
        {
            try
            {
                var screenshotDir = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "Screenshots"
                );
                Directory.CreateDirectory(screenshotDir);

                var fileName = $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(screenshotDir, fileName);

                if (_mainWindow != null)
                {
                    ScreenshotHelper.TakeScreenshot(_mainWindow, filePath);
                    TestContext.AddTestAttachment(filePath, "失敗截圖");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"截圖失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得應用程式路徑
        /// </summary>
        private string GetApplicationPath()
        {
            // 從環境變數或配置檔讀取
            var appPath = Environment.GetEnvironmentVariable("SHOPPING_CART_APP_PATH");

            if (string.IsNullOrEmpty(appPath))
            {
                // 預設路徑（請根據實際情況修改）
                appPath = @"C:\Path\To\ShoppingCart\index.html";
            }

            return appPath;
        }
    }
}
