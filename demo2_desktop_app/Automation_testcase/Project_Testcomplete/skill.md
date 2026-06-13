---
name: demo2-testcomplete-to-flaui
description: >-
  將 Demo2 Desktop App 的 TestComplete Python 測試腳本轉換為 FlaUI C# BDD（SpecFlow +
  Page Object）。當使用者要 TestComplete 轉 FlaUI、BDD 轉換、SpecFlow feature、
  Testcase_demo2_desktop 轉 C#、或提及 Transfer_Prompt / skill.md 時使用。
---

# Demo2：TestComplete → FlaUI BDD 轉換

## 適用範圍

| 項目 | 路徑 |
|------|------|
| 來源腳本 | `Testcomplete_testcase/Testcase_demo2_desktop.md` |
| 規格 | `../Test_cases/SemiInspection_10_TestCases.md` |
| 詳細範例 | [Transfer_Prompt_Readme.md](Transfer_Prompt_Readme.md) |
| 測試報告 | [report_prompt.md](report_prompt.md) |

**應用程式常數（轉換時寫入設定檔，勿硬編在步驟內）：**

- 程序名稱：`Demo2Desktop`
- 視窗標題：`Demo2 Desktop App`
- EXE：`demo2_desktop_app\Demo2Desktop\bin\Debug\Demo2Desktop.exe`
- 測試資料：`demo2_desktop_app\Test_data`

## 執行流程（Agent 依序完成）

1. **讀取** `Testcase_demo2_desktop.md` 全部 `testcase_*` 與 `control_*`。
2. **對照** `SemiInspection_10_TestCases.md` 確認 TC01–TC10 標題與預期行為一致。
3. **產出** FlaUI BDD 專案（建議目錄：`Automation_testcase/Project_FlaUIBDD/`），含下列檔案。
4. **驗證**：每個 Scenario 對應一個 `testcase_tc**`；每個 `control_*` 有對應 Step 或 Page 方法。
5. **摘要**：列出尚未對應的 TestComplete API 或需手動 Name Mapping 的 UI。

## 輸出結構（Demo2 命名）

```
Project_FlaUIBDD/
├── Features/Demo2Desktop.feature
├── StepDefinitions/Demo2DesktopSteps.cs
├── Pages/
│   ├── MainWindowPage.cs      # control_app, control_toolbar, control_menu_shortcut
│   ├── FileDialogPage.cs      # control_file_dialog
│   ├── MessageBoxPage.cs      # control_message_box
│   └── WorkspacePage.cs       # control_desktop (tree, grid, chart, tabs)
├── Hooks/TestHooks.cs
├── Support/BasePage.cs, AppConfig.cs
└── run-tests-and-report.ps1   # 對齊 report_prompt.md
```

## 測試案例對照（必須 1:1）

| TC | testcase 函數 | 標籤建議 |
|----|---------------|----------|
| TC01 | `testcase_tc01_import_excel` | @Functional @Import |
| TC02 | `testcase_tc02_file_tree` | @Functional @FileTree |
| TC03 | `testcase_tc03_data_table` | @Functional @DataTable |
| TC04 | `testcase_tc04_draw_chart` | @Functional @Chart |
| TC05 | `testcase_tc05_file_tree_open_excel` | @Functional @FileTree |
| TC06 | `testcase_tc06_about` | @Functional @About |
| TC07 | `testcase_tc07_invalid_import` | @Negative |
| TC08 | `testcase_tc08_chart_no_data` | @Negative @Chart |
| TC09 | `testcase_tc09_missing_file` | @Negative |
| TC10 | `testcase_tc10_excel_format_compat` | @Compatibility |

`control_run_all_tests()` → Feature 背景或 `@Smoke` 整合 Scenario，或 NUnit `[Test]` 呼叫全部 Scenario。

## control_* 分組（Page Object）

| 函數 | 職責 | FlaUI 類別 |
|------|------|------------|
| `control_app` | 啟動/關閉/主視窗/標題/按鍵 | `MainWindowPage` + `TestHooks` |
| `control_toolbar` | Import Excel、Data Table、Draw data、About | `MainWindowPage` |
| `control_menu_shortcut` | ^i / ^e / ^d | `MainWindowPage` |
| `control_file_dialog` | 開啟檔案對話框 | `FileDialogPage` |
| `control_message_box` | MessageBox 驗證 | `MessageBoxPage` |
| `control_desktop` | TreeView、DataGridView、圖表、分頁 | `WorkspacePage` |
| `control_prepare_testdata` / `control_reset_app_state` | 前置 | `TestHooks` 或 `Given` 步驟 |

## 與 FlaUI 撰寫標準 skill 的關係

- **TestComplete → FlaUI 轉換（本檔）：** 從 Python 腳本產出 BDD 骨架
- **FlaUI 維護與新增 TC：** 用 `SQA_AI_Automation/.cursor/skills/flaui-desktop-bdd/SKILL.md`（Page Object、WinForms UIA、LogContains、FileDialog 等實戰規範）

TestComplete 腳本使用 **WinForms / Desktop**，轉 FlaUI 時優先：

| TestComplete | FlaUI |
|--------------|-------|
| `Sys.Process("Demo2Desktop")` | `Application.Attach` 或 `Application.Launch` |
| `Find(["WndCaption", APP_TITLE])` | `cf.ByName(APP_TITLE)` 或主視窗 `AutomationId` |
| `FindChild("WndCaption", button_text)` | `cf.ByName(button_text)` on ToolStrip |
| `ClrClassName` 如 `TreeView`, `DataGridView` | `cf.ByClassName(...)` |
| `WndCaption` 萬用字串 | `cf.ByName` + `Contains` 或正則 |
| `control_app("keys", "^i")` | `Keyboard.TypeSimultaneously` 或選單 `Invoke` |
| `Log.Error` / 回傳 `False` | `Assert.That` / `Assert.Fail` |
| `control_desktop("screenshot")` | `Capture.Screen().ToFile` 於 AfterScenario |

工具列按鈕文字（與腳本一致）：`Import Excel`、`Data Table`、`Draw data`、`About`。

## 轉換任務（依需求擇一或全套）

### 1. Feature File

- 每個 `testcase_tc**` → 一個 `Scenario`。
- Gherkin：`Given` 啟動 App 與測試資料、`When` 工具列/樹狀操作、`Then` Grid/圖表/MessageBox 驗證。
- 語言：繁體中文（與 `SemiInspection_10_TestCases.md` 一致）。
- 加上 `@Functional` / `@Negative` / `@Compatibility`。

### 2. Step Definitions

- `[Binding]` 類別 `Demo2DesktopSteps`。
- `ScenarioContext` 保存匯入檔路徑、選取的樹節點等。
- 以 FlaUI UIA3 取代 `FindChild` / `Click` / `WaitProperty`。
- 失敗時截圖至 `TestResults/Screenshots/`。

### 3. Page Object Model

- 每個 `control_*` 群組一個 Page；方法名對應 action（如 `ClickToolbar("Import Excel")`）。
- `BasePage`：`FindElement`、`WaitUntilEnabled`、共用 `Window`。
- 實作 `IDisposable` 釋放 `UIA3Automation`（若 Steps 未集中管理）。

### 4. 測試基礎架構

- `BeforeScenario`：`control_prepare_testdata` 等價邏輯 + `Application.Launch(EXE_PATH)`。
- `AfterScenario`：關閉程序、失敗截圖。
- `AppConfig`：EXE、`TEST_DATA_DIR`、逾時（腳本 `TIMEOUT_MS = 30000`）。

### 5. 斷言

- `control_desktop("verify_grid_rows", ...)` → `Assert.That(rowCount, Is.GreaterThan(0))`。
- MessageBox 子字串 → `Assert.That(dialog.Name, Does.Contain(expected))`。

### 6. 完整轉換（預設）

一次產出 §輸出結構 全部檔案；技術棧：

- SpecFlow 3.x+、FlaUI 4.x+、NUnit 3.x+、**.NET 6+**（被測 App 仍為 .NET 3.5 WinForms）。
- 支援平行執行時以獨立程序實例或 `[Parallelizable]` + 獨立 Test_data 複本。

## 快速 API 對照

| 功能 | TestComplete | FlaUI |
|------|-------------|-------|
| 啟動 | `Runner.Run()` | `Application.Launch` |
| 找子元素 | `FindChild("WndCaption", x)` | `FindFirstDescendant(cf.ByName(x))` |
| 點擊 | `.Click()` | `.Click()` |
| 文字 | `.WndCaption` / Grid 儲存格 | `.Name` / `Grid` API |
| 等待 | `WaitProperty("Visible", True)` | `WaitUntilEnabled` / `WaitUntilClickable` |

## 完成檢查清單

- [ ] 10 個 Scenario 與 TC01–TC10 對齊
- [ ] 所有 `control_*` action 有實作或標註 TODO
- [ ] EXE / Test_data 路徑可設定
- [ ] 負向案例（TC07–TC09）驗證 MessageBox 或錯誤狀態
- [ ] TC10 相容性涵蓋 xlsx 格式
- [ ] 說明如何執行 `run-tests-and-report.ps1` 產報告

## 延伸閱讀

- 購物車 Web 版參考：`demo1_shoppingcart_website/.../Testcase_shopping_cart.md`
- 長篇轉換範例與 FAQ：[Transfer_Prompt_Readme.md](Transfer_Prompt_Readme.md)
