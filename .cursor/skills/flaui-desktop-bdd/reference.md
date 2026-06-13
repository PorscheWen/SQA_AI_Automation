# FlaUI Desktop BDD — 參考模板

## Feature 全文範本（10 TC）

路徑：`demo2_desktop_app/Automation_testcase/Project_FlaUIBDD/Testcase_demo2_desktop_FlaUI_BDD/Features/Demo2Desktop.feature`

```gherkin
Feature: Semi Inspection Desktop 測試
  作為測試人員
  我想要驗證 Semi Inspection Desktop 的 Recipe、RawData 與圖表功能
  以確保 Recipe_data 工作流程正常

@Functional @Import
Scenario: TC01 - Import Recipe 至 Recipe_data
  Given 測試資料已就緒
  And 應用程式已重新啟動
  When 我點擊工具列「Import Recipe」
  And 我在檔案對話框選擇樣本 InspectionRecipe_Sample.json
  Then Recipe_data 應存在 InspectionRecipe_Sample.json
  And 資料表應可見
  And 日誌區應包含「Import Recipe」

@Functional @About
Scenario: TC06 - About 對話框
  Given 應用程式已啟動
  When 我點擊工具列「About」
  And 我關閉訊息對話框
```

## StepDefinitions 常用步驟

```csharp
[When(@"我點擊工具列「(.*)」")]
public void When我點擊工具列(string buttonText)
{
    if (string.Equals(buttonText, "Run Inspection", StringComparison.OrdinalIgnoreCase))
    {
        Main.SendShortcut(VirtualKeyShort.KEY_R, ctrl: true);
        Thread.Sleep(800);
        return;
    }
    Main.ClickToolbar(buttonText);
    // 依按鈕追加等待...
}

[Then(@"日誌區應包含「(.*)」")]
public void Then日誌區應包含(string expected)
{
    ClassicAssert.IsTrue(
        Workspace.LogContains(expected),
        "日誌區未包含預期文字: " + expected);
}

[When(@"我關閉訊息對話框")]
[Then(@"我關閉訊息對話框")]
public void When我關閉訊息對話框() => MessageBox.ClickOk();
```

## MainWindowPage — 工具列對照表

```csharp
private static readonly Dictionary<string, (string AutomationId, VirtualKeyShort? Shortcut)> ToolbarMap =
    new(StringComparer.OrdinalIgnoreCase)
    {
        ["Import Recipe"] = ("btnImportRecipe", VirtualKeyShort.KEY_I),
        ["About"] = ("btnToolbar0About", null),
        ["RawData"] = ("btnParameters", VirtualKeyShort.KEY_E),
        ["Defect Chart"] = ("btnDefectChart", VirtualKeyShort.KEY_D),
        ["Run Inspection"] = ("btnRunInspection", VirtualKeyShort.KEY_R),
    };
```

Import Recipe：`SendImportRecipeShortcut()`（Ctrl+I），**不要**先 Invoke ToolStrip（易 NoClickablePoint）。

About：`OpenAboutViaKeyboard()` — F10 → RIGHT（Tools）→ DOWN → DOWN×4 → RETURN。

## FileDialogPage — 核心模式

1. 建構子接收 `Window owner`，搜尋 root = owner + desktop。
2. `FindOpenDialog`：先找 AutomationId `1148`，再 `#32770` fallback。
3. 找不到 dialog：聚焦主視窗 → `Alt+D` / `Alt+N` 鍵入完整路徑 → Enter。
4. 路徑用 `ConfigHelper.GetSampleRecipePath()`，不在 Step 拼字串。

## WorkspacePage — LogContains

```csharp
public bool LogContains(string expected, int waitMs = 10000)
{
    var deadline = DateTime.UtcNow.AddMilliseconds(waitMs);
    while (DateTime.UtcNow < deadline)
    {
        if (TryReadLogContains(expected)) return true;
        Thread.Sleep(250);
    }
    return false;
}
```

讀取順序：

1. `txtToolLog`、`lblToolPlugin`、`statusLabel`（FindWinFormsControl）
2. 全 descendant 的 Name / WM_GETTEXT / ReadText

BasePage 提供 `ReadNativeWindowText`（user32 `WM_GETTEXT`），供 .NET 3.5 WinForms TextBox 使用。

## 被測 App — 自動化友善 statusLabel

```csharp
// MainForm.cs 範例
statusLabel.Text = "RawData: parameters view";
statusLabel.Text = "Import Recipe: " + Path.GetFileName(fullDest);
statusLabel.Text = "Defect Chart: " + chartDefectTypes.Count + " types";
statusLabel.Text = "Run Inspection: complete - " + currentRecipe.RecipeName;
```

`AppendToolLog` 可同步更新 `txtToolLog.AccessibleName`（最後 512 字元）。

## TestHooks 注入

```csharp
_scenarioContext.Set(new MainWindowPage(_mainWindow, _automation!), "MainWindowPage");
_scenarioContext.Set(new WorkspacePage(_mainWindow, _automation!), "WorkspacePage");
_scenarioContext.Set(new FileDialogPage(_automation!, _mainWindow), "FileDialogPage");
_scenarioContext.Set(new MessageBoxPage(_automation!), "MessageBoxPage");
```

## 案例表 Markdown 列模板

```markdown
| TC11 | Functional | 新功能 | 操作摘要 | 預期結果 | 2011 |
```

同步更新 `generate_semi_testcases.py` 產生 xlsx（若專案有）。

## 常見失敗與處置

| 症狀 | 原因 | 處置 |
|------|------|------|
| File open dialog not found | Win11 對話框不在 desktop 第一層 | owner 子樹 + 1148 + 鍵盤 fallback |
| NoClickablePointException | ToolStrip 無 Invoke 點 | 快捷鍵或 bounds click |
| UIA Timeout About | btnToolbar0About Invoke 慢 | F10 選單 |
| 日誌區未包含 | UIA 讀不到 TextBox | WM_GETTEXT + statusLabel 關鍵字 |
| Then 我關閉訊息對話框 pending | 只有 When binding | 加上 `[Then]` 同方法 |
| TC08 狀態污染 | 前案未關 app | AfterScenario kill + RelaunchApplication |
| Application failed to exit | 對話框未關 | DialogHelper.DismissOpenDialogs |

## Inspector 驗證

```bat
demo2_desktop_app\Automation_testcase\Project_FlaUIBDD\Testcase_demo2_desktop_FlaUI_BDD\開啟Inspector.bat
```

確認行程 `SemiInspectionDesktop` 下 AutomationId 與 `SemiInspection_10_TestCases.md` 一致。

## Web Dashboard（可選）

```bat
Automation_testcase\Project_FlaUIBDD\web_dashboard\開啟控制台.bat
```

執行篩選範例：`Name~TC01|Name~TC03`，結果寫入 `web_dashboard/data/status.json`。
