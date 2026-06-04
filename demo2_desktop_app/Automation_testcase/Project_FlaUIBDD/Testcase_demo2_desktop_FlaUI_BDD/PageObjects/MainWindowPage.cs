using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;

namespace Demo2DesktopTests.PageObjects;

public class MainWindowPage : BasePage
{
    private const int ToolbarLookupMs = 2500;

    private static readonly Dictionary<string, (string AutomationId, VirtualKeyShort? Shortcut)> ToolbarMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Import Excel"] = ("btnToolbar0ImportExcel", VirtualKeyShort.KEY_I),
            ["About"] = ("btnToolbar0About", null),
            ["Data Table"] = ("btnToolbar1OpenExcel", VirtualKeyShort.KEY_E),
            ["Draw data"] = ("btnToolbar2DrawData", VirtualKeyShort.KEY_D),
        };

    public MainWindowPage(Window window, UIA3Automation automation) : base(window, automation) { }

    public void ClickToolbar(string buttonText)
    {
        FocusMainWindow();

        if (ToolbarMap.TryGetValue(buttonText, out var mapped))
        {
            // 快捷鍵優先（避免 ToolStrip UIA 搜尋逾時，尤其 Import Excel）
            if (mapped.Shortcut.HasValue && TryInvokeShortcut(mapped.Shortcut.Value))
            {
                return;
            }

            if (TryClickToolbarElement(mapped.AutomationId, buttonText))
            {
                return;
            }
        }
        else if (TryClickToolbarElement(null, buttonText))
        {
            return;
        }

        throw new ElementNotAvailableException($"找不到工具列按鈕: {buttonText}");
    }

    public void SendShortcut(VirtualKeyShort key, bool ctrl = false)
    {
        FocusMainWindow();
        if (ctrl)
        {
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, key);
        }
        else
        {
            Keyboard.Type(key);
        }

        Thread.Sleep(500);
    }

    public void SendDataTableShortcut() => SendShortcut(VirtualKeyShort.KEY_E, ctrl: true);

    public void SendImportExcelShortcut() => SendShortcut(VirtualKeyShort.KEY_I, ctrl: true);

    public string GetWindowTitle() => Window.Title;

    public bool IsMainWindowVisible() => Window.IsAvailable;

    private void FocusMainWindow()
    {
        try
        {
            Window.Focus();
            Window.SetForeground();
        }
        catch
        {
            // 部分環境無法強制置前
        }

        Thread.Sleep(300);
    }

    private bool TryClickToolbarElement(string? automationId, string displayText)
    {
        if (!string.IsNullOrEmpty(automationId))
        {
            var byId = FindByAutomationId(automationId, ToolbarLookupMs);
            if (byId != null)
            {
                Click(byId);
                return true;
            }
        }

        var byName = FindByName(displayText, ToolbarLookupMs);
        if (byName != null)
        {
            Click(byName);
            return true;
        }

        var byPartial = FindToolbarButtonByText(displayText, ToolbarLookupMs);
        if (byPartial != null)
        {
            Click(byPartial);
            return true;
        }

        return false;
    }

    private AutomationElement? FindToolbarButtonByText(string text, int timeoutMs)
    {
        var el = FindWinFormsControl("btnToolbar0ImportExcel");
        if (el != null && text.Contains("Import", StringComparison.OrdinalIgnoreCase))
        {
            return el;
        }

        el = FindWinFormsControl("btnToolbar1OpenExcel");
        if (el != null && text.Contains("Data", StringComparison.OrdinalIgnoreCase))
        {
            return el;
        }

        el = FindWinFormsControl("btnToolbar2DrawData");
        if (el != null && text.Contains("Draw", StringComparison.OrdinalIgnoreCase))
        {
            return el;
        }

        el = FindWinFormsControl("btnToolbar0About");
        if (el != null && text.Contains("About", StringComparison.OrdinalIgnoreCase))
        {
            return el;
        }

        return null;
    }

    private static bool TryInvokeShortcut(VirtualKeyShort key)
    {
        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, key);
        Thread.Sleep(800);
        return true;
    }
}
