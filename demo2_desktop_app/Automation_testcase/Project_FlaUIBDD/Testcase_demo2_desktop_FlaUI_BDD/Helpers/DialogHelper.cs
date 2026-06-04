using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;

namespace Demo2DesktopTests.Helpers;

/// <summary>關閉可能阻擋 UIA 的對話框，避免 TearDown 逾時。</summary>
public static class DialogHelper
{
    public static void DismissOpenDialogs(UIA3Automation? automation, int escapePresses = 4)
    {
        try
        {
            for (var i = 0; i < escapePresses; i++)
            {
                Keyboard.Press(VirtualKeyShort.ESCAPE);
                Thread.Sleep(250);
            }
        }
        catch
        {
            // ignore
        }

        if (automation == null)
        {
            return;
        }

        try
        {
            var desktop = automation.GetDesktop();
            foreach (var w in desktop.FindAllChildren(cf => cf.ByClassName("#32770")))
            {
                try
                {
                    w.AsWindow()?.Close();
                }
                catch
                {
                    // ignore
                }
            }
        }
        catch
        {
            // ignore
        }
    }
}
