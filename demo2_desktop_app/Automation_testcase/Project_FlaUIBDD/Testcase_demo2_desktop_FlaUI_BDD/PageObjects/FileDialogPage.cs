using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;

namespace Demo2DesktopTests.PageObjects;

public class FileDialogPage
{
    private readonly UIA3Automation _automation;

    public FileDialogPage(UIA3Automation automation)
    {
        _automation = automation;
    }

    /// <summary>
    /// 以鍵盤操作開啟檔案（避免 TextBox.Enter 造成 UIA Timeout）。
    /// </summary>
    public void OpenFile(string fullPath, int timeoutMs = 12000)
    {
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Sample file not found: " + fullPath);
        }

        var dialog = WaitForOpenDialog(timeoutMs);
        if (dialog == null)
        {
            throw new InvalidOperationException("File open dialog not found");
        }

        FocusDialogSafe(dialog);
        Thread.Sleep(500);

        // 優先：檔名欄位（AutomationId 1148）僅設定焦點後用鍵盤輸入，不用 Enter()
        if (!TryTypePathInFileNameField(dialog, fullPath))
        {
            TypePathWithKeyboardOnly(fullPath);
        }

        Thread.Sleep(200);
        Keyboard.Press(VirtualKeyShort.RETURN);
        WaitForDialogClosed(10000);
        Thread.Sleep(800);
    }

    private static void FocusDialogSafe(Window dialog)
    {
        try
        {
            dialog.SetForeground();
            dialog.Focus();
        }
        catch
        {
            // ignore
        }
    }

    private static bool TryTypePathInFileNameField(Window dialog, string fullPath)
    {
        try
        {
            var fileNameBox = dialog.FindFirstDescendant(cf => cf.ByAutomationId("1148"))
                ?? dialog.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));

            if (fileNameBox == null)
            {
                return false;
            }

            fileNameBox.Focus();
            Thread.Sleep(150);
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            Thread.Sleep(50);
            Keyboard.Type(fullPath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void TypePathWithKeyboardOnly(string fullPath)
    {
        // 開啟對話框：Alt+D 聚焦位址列（Win10+），或 Alt+N 檔名欄
        try
        {
            Keyboard.TypeSimultaneously(VirtualKeyShort.ALT, VirtualKeyShort.KEY_D);
            Thread.Sleep(200);
        }
        catch
        {
            Keyboard.TypeSimultaneously(VirtualKeyShort.ALT, VirtualKeyShort.KEY_N);
            Thread.Sleep(200);
        }

        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
        Thread.Sleep(50);
        Keyboard.Type(fullPath);
    }

    private Window? WaitForOpenDialog(int timeoutMs)
    {
        return Retry.WhileNull(FindOpenDialog, TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    private void WaitForDialogClosed(int timeoutMs)
    {
        Retry.WhileTrue(
            () => FindOpenDialog() != null,
            TimeSpan.FromMilliseconds(timeoutMs));
    }

    private Window? FindOpenDialog()
    {
        try
        {
            var desktop = _automation.GetDesktop();
            Window? fallback = null;

            foreach (var w in desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window)))
            {
                var win = w.AsWindow();
                if (win == null || string.IsNullOrEmpty(win.Title))
                {
                    continue;
                }

                var title = win.Title;
                if (title.Contains("Import Excel", StringComparison.OrdinalIgnoreCase) ||
                    title.Contains("開啟", StringComparison.OrdinalIgnoreCase) ||
                    title.Contains("Open", StringComparison.OrdinalIgnoreCase))
                {
                    return win;
                }

                if (win.ClassName == "#32770" && fallback == null)
                {
                    fallback = win;
                }
            }

            return fallback;
        }
        catch
        {
            return null;
        }
    }
}
