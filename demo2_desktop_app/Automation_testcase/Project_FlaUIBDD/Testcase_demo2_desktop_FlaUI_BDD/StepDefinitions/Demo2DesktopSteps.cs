using Demo2DesktopTests.Helpers;
using Demo2DesktopTests.Hooks;
using Demo2DesktopTests.PageObjects;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using TechTalk.SpecFlow;

namespace Demo2DesktopTests.StepDefinitions;

[Binding]
public class Demo2DesktopSteps
{
    private readonly ScenarioContext _scenarioContext;

    public Demo2DesktopSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private MainWindowPage Main => _scenarioContext.Get<MainWindowPage>("MainWindowPage");
    private WorkspacePage Workspace => _scenarioContext.Get<WorkspacePage>("WorkspacePage");
    private FileDialogPage FileDialog => _scenarioContext.Get<FileDialogPage>("FileDialogPage");
    private MessageBoxPage MessageBox => _scenarioContext.Get<MessageBoxPage>("MessageBoxPage");

    [Given(@"測試資料已就緒")]
    public void Given測試資料已就緒()
    {
        TestDataHelper.EnsureTestDataReady();
    }

    [Given(@"應用程式已啟動")]
    public void Given應用程式已啟動()
    {
        ClassicAssert.IsTrue(Main.IsMainWindowVisible(), "主視窗應已啟動");
        Thread.Sleep(1000);
    }

    [Given(@"應用程式已重新啟動")]
    public void Given應用程式已重新啟動()
    {
        TestHooks.RelaunchApplication();
        TestHooks.BindPagesToScenario(_scenarioContext);
        ClassicAssert.IsTrue(Main.IsMainWindowVisible());
    }

    [When(@"我點擊工具列「(.*)」")]
    public void When我點擊工具列(string buttonText)
    {
        if (string.Equals(buttonText, "Import Excel", StringComparison.OrdinalIgnoreCase))
        {
            Main.SendImportExcelShortcut();
            Thread.Sleep(800);
            return;
        }

        Main.ClickToolbar(buttonText);
        if (string.Equals(buttonText, "Data Table", StringComparison.OrdinalIgnoreCase))
        {
            Workspace.WaitAfterDataTableAction();
        }
        else if (string.Equals(buttonText, "Draw data", StringComparison.OrdinalIgnoreCase))
        {
            Thread.Sleep(1500);
        }
        else
        {
            Thread.Sleep(500);
        }
    }

    [When(@"我在檔案對話框選擇樣本 X\.xlsx")]
    public void When我在檔案對話框選擇樣本Xlsx()
    {
        FileDialog.OpenFile(ConfigHelper.GetSampleXlsxPath());
        Workspace.WaitAfterDataTableAction();
    }

    [When(@"我在檔案對話框選擇無效檔 _invalid_sample\.txt")]
    public void When我在檔案對話框選擇無效檔()
    {
        FileDialog.OpenFile(ConfigHelper.GetInvalidSamplePath());
    }

    [When(@"我在檔案樹雙擊 X\.xlsx 或 Demo2_10 測試檔")]
    public void When我在檔案樹雙擊測試檔()
    {
        Workspace.DoubleClickTreeItem("X.xlsx");
        if (!Workspace.IsGridVisible())
        {
            Workspace.DoubleClickTreeItem("Demo2_10");
        }
        if (!Workspace.IsGridVisible())
        {
            Main.ClickToolbar("Data Table");
        }
    }

    [When(@"我關閉訊息對話框")]
    public void When我關閉訊息對話框()
    {
        MessageBox.ClickOk();
    }

    [When(@"我使用快捷鍵開啟 Data Table 並選擇不存在檔 not_exist_99999\.xlsx")]
    public void When我使用快捷鍵開啟不存在檔()
    {
        Main.SendDataTableShortcut();
        Thread.Sleep(500);
        var missing = Path.Combine(ConfigHelper.GetTestDataDirectory(), "not_exist_99999.xlsx");
        try
        {
            FileDialog.OpenFile(missing);
            MessageBox.ClickOk();
        }
        catch
        {
            // 對話框可能未出現，與 TestComplete 腳本一致仍視為通過前置
        }
    }

    [Then(@"Test_data 應存在 X\.xlsx")]
    public void ThenTest_data應存在Xlsx()
    {
        ClassicAssert.IsTrue(
            File.Exists(ConfigHelper.GetSampleXlsxPath()),
            "Test_data 應存在 X.xlsx");
    }

    [Then(@"主視窗標題應為「(.*)」")]
    public void Then主視窗標題應為(string expected)
    {
        ClassicAssert.AreEqual(expected, Main.GetWindowTitle());
    }

    [Then(@"檔案樹應可見")]
    public void Then檔案樹應可見()
    {
        ClassicAssert.IsTrue(
            Workspace.IsTreeVisible(),
            "找不到 File Tree（請確認 treeFiles / SysTreeView32 可被 UIA 存取）");
    }

    [Then(@"資料表應可見")]
    public void Then資料表應可見()
    {
        ClassicAssert.IsTrue(
            Workspace.IsGridVisible(),
            "找不到 DataGridView（請確認 dataGridExcel 已載入且 Tab 在資料表）");
    }

    [Then(@"日誌區應包含「(.*)」")]
    public void Then日誌區應包含(string expected)
    {
        ClassicAssert.IsTrue(
            Workspace.LogContains(expected),
            "日誌區未包含預期文字: " + expected + "（檢查 txtToolLog / lblToolPlugin）");
    }

    [Then(@"不應將無效檔複製為 TC01_import_copy\.xlsx")]
    public void Then不應將無效檔複製為匯入檔()
    {
        ClassicAssert.IsFalse(
            TestDataHelper.ImportTargetExists(),
            "無效檔不應被複製為 TC01_import_copy.xlsx");
    }

    [Then(@"主視窗仍應存在")]
    public void Then主視窗仍應存在()
    {
        ClassicAssert.IsTrue(Main.IsMainWindowVisible());
    }

}
