using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.Core.Exceptions;
using FlaUI.UIA3;

namespace ShoppingCartTests.PageObjects
{
    /// <summary>
    /// 基礎頁面類別，提供所有頁面共用的功能
    /// </summary>
    public abstract class BasePage : IDisposable
    {
        protected readonly Window Window;
        protected readonly UIA3Automation Automation;
        private readonly int DefaultTimeout = 10000; // 毫秒

        protected BasePage(Window window, UIA3Automation automation)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
            Automation = automation ?? throw new ArgumentNullException(nameof(automation));
        }

        /// <summary>
        /// 根據 AutomationId 尋找元素
        /// </summary>
        protected AutomationElement FindElement(string automationId, int timeout = 0)
        {
            if (timeout == 0) timeout = DefaultTimeout;

            try
            {
                var element = Retry.WhileNull(() =>
                    Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
                    TimeSpan.FromMilliseconds(timeout)
                ).Result;

                if (element == null)
                {
                    throw new ElementNotAvailableException($"找不到元素: AutomationId={automationId}");
                }

                return element;
            }
            catch (Exception ex)
            {
                throw new ElementNotAvailableException($"尋找元素失敗: AutomationId={automationId}", ex);
            }
        }

        /// <summary>
        /// 根據 Name 尋找元素
        /// </summary>
        protected AutomationElement FindElementByName(string name, int timeout = 0)
        {
            if (timeout == 0) timeout = DefaultTimeout;

            try
            {
                var element = Retry.WhileNull(() =>
                    Window.FindFirstDescendant(cf => cf.ByName(name)),
                    TimeSpan.FromMilliseconds(timeout)
                ).Result;

                if (element == null)
                {
                    throw new ElementNotAvailableException($"找不到元素: Name={name}");
                }

                return element;
            }
            catch (Exception ex)
            {
                throw new ElementNotAvailableException($"尋找元素失敗: Name={name}", ex);
            }
        }

        /// <summary>
        /// 點擊元素
        /// </summary>
        protected void ClickElement(string automationId, int timeout = 0)
        {
            var element = FindElement(automationId, timeout);
            element.AsButton()?.Click();
            WaitForWindow();
        }

        /// <summary>
        /// 取得元素文字
        /// </summary>
        protected string GetElementText(string automationId, int timeout = 0)
        {
            var element = FindElement(automationId, timeout);
            return element.Name ?? element.AsLabel()?.Text ?? string.Empty;
        }

        /// <summary>
        /// 設定文字輸入
        /// </summary>
        protected void SetElementText(string automationId, string text, int timeout = 0)
        {
            var element = FindElement(automationId, timeout);
            var textBox = element.AsTextBox();
            if (textBox != null)
            {
                textBox.Text = text;
            }
        }

        /// <summary>
        /// 驗證元素文字
        /// </summary>
        protected bool VerifyElementText(string automationId, string expectedText, int timeout = 0)
        {
            try
            {
                var actualText = GetElementText(automationId, timeout);
                return actualText == expectedText;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查元素是否存在
        /// </summary>
        protected bool IsElementExists(string automationId, int timeout = 1000)
        {
            try
            {
                var element = FindElement(automationId, timeout);
                return element != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 等待元素出現
        /// </summary>
        protected void WaitForElement(string automationId, int timeout = 0)
        {
            FindElement(automationId, timeout);
        }

        /// <summary>
        /// 等待視窗回應
        /// </summary>
        protected void WaitForWindow()
        {
            // Wait for window to be responsive
            // Wait.UntilResponsive(Window, TimeSpan.FromSeconds(5));
            System.Threading.Thread.Sleep(500);
        }

        /// <summary>
        /// 截圖
        /// </summary>
        public void TakeScreenshot(string filePath)
        {
            try
            {
                var screenshot = Window.Capture();
                screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine($"截圖已儲存: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"截圖失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public virtual void Dispose()
        {
            // 子類別可以覆寫此方法來清理資源
        }
    }
}
