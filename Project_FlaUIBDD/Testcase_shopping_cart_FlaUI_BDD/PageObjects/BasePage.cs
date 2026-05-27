using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                var element = Retry.WhileNull(
                    () => FindElementInternal(automationId),
                    TimeSpan.FromMilliseconds(timeout)
                ).Result;

                if (element == null)
                {
                    throw new ElementNotAvailableException($"找不到元素: AutomationId={automationId}");
                }

                return element;
            }
            catch (ElementNotAvailableException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ElementNotAvailableException($"尋找元素失敗: AutomationId={automationId}", ex);
            }
        }

        private AutomationElement FindElementInternal(string automationId)
        {
            foreach (var root in GetSearchRoots())
            {
                var byAutomationId = root.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
                if (byAutomationId != null)
                {
                    return byAutomationId;
                }

                var byName = root.FindFirstDescendant(cf => cf.ByName(automationId));
                if (byName != null)
                {
                    return byName;
                }

                var byFrameworkId = root.FindFirstDescendant(cf => cf.ByFrameworkId(automationId));
                if (byFrameworkId != null)
                {
                    return byFrameworkId;
                }
                foreach (var element in root.FindAllDescendants())
                {
                    if (string.Equals(element.Properties.AutomationId.ValueOrDefault, automationId, StringComparison.Ordinal))
                    {
                        return element;
                    }
                }
            }

            return null;
        }

        protected AutomationElement FindElementInRoots(string automationId)
        {
            foreach (var root in GetSearchRoots())
            {
                var element = root.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
                if (element != null)
                {
                    return element;
                }

                foreach (var descendant in root.FindAllDescendants())
                {
                    if (string.Equals(descendant.Properties.AutomationId.ValueOrDefault, automationId, StringComparison.Ordinal))
                    {
                        return descendant;
                    }
                }
            }

            return null;
        }

        protected static bool IsDigitsOnly(string text)
        {
            return text.Length > 0 && text.All(char.IsDigit);
        }

        protected IEnumerable<AutomationElement> GetSearchRoots()
        {
            var document = Window.FindFirstDescendant(cf => cf.ByControlType(ControlType.Document));
            if (document != null)
            {
                yield return document;
            }

            yield return Window;

            var pane = Window.FindFirstDescendant(cf => cf.ByControlType(ControlType.Pane));
            if (pane != null)
            {
                yield return pane;
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

            if (element.Patterns.Invoke.IsSupported)
            {
                element.Patterns.Invoke.Pattern.Invoke();
            }
            else
            {
                element.Click();
            }

            WaitForWindow();
        }

        /// <summary>
        /// 取得元素文字
        /// </summary>
        protected string GetElementText(string automationId, int timeout = 0)
        {
            var element = FindElement(automationId, timeout);
            return ReadElementText(element);
        }

        protected string GetTextMatchingPattern(string pattern, int timeout = 0)
        {
            if (timeout == 0) timeout = DefaultTimeout;

            var element = Retry.WhileNull(
                () => FindTextElementByPattern(pattern),
                TimeSpan.FromMilliseconds(timeout)
            ).Result;

            if (element == null)
            {
                throw new ElementNotAvailableException($"找不到符合文字模式 '{pattern}' 的元素");
            }

            return ReadElementText(element);
        }

        protected string GetCartCountText(int timeout = 0)
        {
            if (timeout == 0) timeout = DefaultTimeout;

            var element = Retry.WhileNull(
                () => FindCartCountElement(),
                TimeSpan.FromMilliseconds(timeout)
            ).Result;

            if (element == null)
            {
                throw new ElementNotAvailableException("找不到購物車件數元素");
            }

            return ReadElementText(element);
        }

        protected string GetLastTextMatchingPattern(string pattern, int timeout = 0)
        {
            if (timeout == 0) timeout = DefaultTimeout;

            var regex = new Regex(pattern, RegexOptions.CultureInvariant);
            AutomationElement lastMatch = null;

            var found = Retry.WhileTrue(
                () =>
                {
                    lastMatch = null;
                    foreach (var root in GetSearchRoots())
                    {
                        foreach (var element in root.FindAllDescendants(cf => cf.ByControlType(ControlType.Text)))
                        {
                            var name = element.Name ?? string.Empty;
                            if (regex.IsMatch(name))
                            {
                                lastMatch = element;
                            }
                        }
                    }

                    return lastMatch == null;
                },
                TimeSpan.FromMilliseconds(timeout)
            );

            if (lastMatch == null)
            {
                throw new ElementNotAvailableException($"找不到符合文字模式 '{pattern}' 的元素");
            }

            return ReadElementText(lastMatch);
        }

        private AutomationElement FindCartCountElement()
        {
            foreach (var root in GetSearchRoots())
            {
                var textElements = root.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));
                foreach (var element in textElements)
                {
                    var name = element.Name ?? string.Empty;
                    if (name.Length >= 2 &&
                        char.IsDigit(name[0]) &&
                        name.EndsWith("件", StringComparison.Ordinal) &&
                        name.Contains(' ', StringComparison.Ordinal))
                    {
                        return element;
                    }
                }
            }

            return null;
        }

        protected bool TryGetElementText(string automationId, out string text, int timeout = 1000)
        {
            text = string.Empty;

            try
            {
                text = GetElementText(automationId, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected static string ReadElementText(AutomationElement element)
        {
            var name = element.Name ?? element.AsLabel()?.Text ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (element.Patterns.Value.IsSupported)
            {
                return element.Patterns.Value.Pattern.Value.Value ?? string.Empty;
            }

            if (element.Patterns.LegacyIAccessible.IsSupported)
            {
                return element.Patterns.LegacyIAccessible.Pattern.Value.Value ?? string.Empty;
            }

            return string.Empty;
        }

        private AutomationElement FindTextElementByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.CultureInvariant);

            foreach (var root in GetSearchRoots())
            {
                var textElements = root.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));
                foreach (var element in textElements)
                {
                    var name = element.Name ?? string.Empty;
                    if (regex.IsMatch(name))
                    {
                        return element;
                    }
                }
            }

            return null;
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
            System.Threading.Thread.Sleep(1000);
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
