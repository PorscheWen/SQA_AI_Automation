using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

namespace Demo2DesktopTests.PageObjects;

public abstract class BasePage : IDisposable
{
    protected readonly Window Window;
    protected readonly UIA3Automation Automation;
    private readonly int _defaultTimeoutMs;

    protected BasePage(Window window, UIA3Automation automation)
    {
        Window = window ?? throw new ArgumentNullException(nameof(window));
        Automation = automation ?? throw new ArgumentNullException(nameof(automation));
        _defaultTimeoutMs = Helpers.ConfigHelper.GetDefaultTimeout();
    }

    protected AutomationElement? FindByName(string name, int timeoutMs = 0)
    {
        timeoutMs = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
        return Retry.WhileNull(
            () => Window.FindFirstDescendant(cf => cf.ByName(name)),
            TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    protected AutomationElement? FindByAutomationId(string automationId, int timeoutMs = 0)
    {
        timeoutMs = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
        return Retry.WhileNull(
            () => Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
            TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    protected AutomationElement? FindByNameContains(string partial, int timeoutMs = 5000)
    {
        return Retry.WhileNull(
            () =>
            {
                foreach (var el in Window.FindAllDescendants())
                {
                    var n = el.Name ?? string.Empty;
                    if (n.Contains(partial, StringComparison.OrdinalIgnoreCase))
                    {
                        return el;
                    }
                }
                return null;
            },
            TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    protected AutomationElement? FindByClassName(string className, int timeoutMs = 0)
    {
        timeoutMs = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
        return Retry.WhileNull(
            () => Window.FindFirstDescendant(cf => cf.ByClassName(className)),
            TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    /// <summary>WinForms 控制項：依 Name / AutomationId 尋找（兩者通常皆為 Designer.Name）。</summary>
    protected AutomationElement? FindWinFormsControl(string controlName)
    {
        var byId = Window.FindFirstDescendant(cf => cf.ByAutomationId(controlName));
        if (byId != null)
        {
            return byId;
        }

        return Window.FindFirstDescendant(cf => cf.ByName(controlName));
    }

    protected static string ReadText(AutomationElement element)
    {
        var textBox = element.AsTextBox();
        if (textBox != null)
        {
            var text = textBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        if (!string.IsNullOrWhiteSpace(element.Name))
        {
            return element.Name;
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

    protected void Click(AutomationElement element)
    {
        if (element.Patterns.Invoke.IsSupported)
        {
            element.Patterns.Invoke.Pattern.Invoke();
        }
        else
        {
            element.Click();
        }

        Thread.Sleep(500);
    }

    protected void RequireElement(AutomationElement? element, string label)
    {
        if (element == null)
        {
            throw new ElementNotAvailableException(label);
        }
    }

    public virtual void Dispose() { }
}
