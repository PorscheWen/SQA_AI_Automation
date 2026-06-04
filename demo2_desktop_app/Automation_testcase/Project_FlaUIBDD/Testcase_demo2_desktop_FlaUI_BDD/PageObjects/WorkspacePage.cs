using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

namespace Demo2DesktopTests.PageObjects;

public class WorkspacePage : BasePage
{
    public WorkspacePage(Window window, UIA3Automation automation) : base(window, automation) { }

    public bool IsTreeVisible(int waitMs = 10000)
    {
        return WaitForTree(waitMs) != null;
    }

    public bool IsGridVisible(int waitMs = 15000)
    {
        return WaitForDataGrid(waitMs) != null;
    }

    public bool LogContains(string expected, int waitMs = 10000)
    {
        var timedOut = Retry.WhileTrue(
            () => !TryReadLogContains(expected),
            TimeSpan.FromMilliseconds(waitMs)).Result;

        return !timedOut;
    }

    public void DoubleClickTreeItem(string partialName)
    {
        var tree = WaitForTree(8000);
        if (tree == null)
        {
            Console.WriteLine("TreeView not found for double-click");
            return;
        }

        foreach (var el in tree.FindAllDescendants())
        {
            var name = el.Name ?? string.Empty;
            if (name.Contains(partialName, StringComparison.OrdinalIgnoreCase))
            {
                el.DoubleClick();
                Thread.Sleep(1500);
                return;
            }
        }

        var fallback = FindByNameContains(partialName, timeoutMs: 3000);
        if (fallback != null)
        {
            fallback.DoubleClick();
            Thread.Sleep(1500);
            return;
        }

        Console.WriteLine($"Tree item not found: {partialName}; caller may use fallback");
    }

    public void TakeScreenshot(string fileName)
    {
        var dir = Path.Combine(AppContext.BaseDirectory, Helpers.ConfigHelper.GetScreenshotDirectory());
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);
        var capture = Window.Capture();
        capture.Save(path);
        Console.WriteLine($"Screenshot: {path}");
    }

    public void WaitAfterDataTableAction()
    {
        WaitForDataGrid(15000);
        Thread.Sleep(800);
    }

    private AutomationElement? WaitForTree(int timeoutMs)
    {
        return Retry.WhileNull(FindTreeView, TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    private AutomationElement? WaitForDataGrid(int timeoutMs)
    {
        return Retry.WhileNull(FindDataGrid, TimeSpan.FromMilliseconds(timeoutMs)).Result;
    }

    private AutomationElement? FindTreeView()
    {
        foreach (var name in new[] { "treeFiles", "fileTree" })
        {
            var el = FindWinFormsControl(name);
            if (el != null)
            {
                return el;
            }
        }

        return Window.FindFirstDescendant(cf => cf.ByControlType(ControlType.Tree))
            ?? Window.FindFirstDescendant(cf => cf.ByClassName("SysTreeView32"))
            ?? Window.FindFirstDescendant(cf => cf.ByClassName("TreeView"));
    }

    private AutomationElement? FindDataGrid()
    {
        var el = FindWinFormsControl("dataGridExcel");
        if (el != null)
        {
            return el;
        }

        return Window.FindFirstDescendant(cf => cf.ByControlType(ControlType.DataGrid))
            ?? Window.FindFirstDescendant(cf => cf.ByClassName("DataGridView"))
            ?? Window.FindFirstDescendant(cf => cf.ByClassName("WindowsForms10.SysDataGridView"));
    }

    private bool TryReadLogContains(string expected)
    {
        var log = FindWinFormsControl("txtToolLog");
        if (log != null)
        {
            var text = ReadText(log);
            if (text.Contains(expected, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        var header = FindWinFormsControl("lblToolPlugin");
        if (header != null)
        {
            var text = ReadText(header);
            if (text.Contains(expected, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        foreach (var edit in Window.FindAllDescendants(cf => cf.ByClassName("TextBox")))
        {
            var text = ReadText(edit);
            if (text.Contains(expected, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
