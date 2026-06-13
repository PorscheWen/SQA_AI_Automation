---
name: flaui-desktop-bdd
description: >-
  撰寫 WinForms 桌面程式的 FlaUI + SpecFlow BDD 自動化測試（Page Object、Feature、StepDefinitions、Hooks）。
  當使用者要新增/修改 FlaUI testcase、SpecFlow feature、Page Object、桌面 UI 自動化、
  Semi Inspection Desktop 測試、或對照 TEST_PLAN / SemiInspection_10_TestCases 時使用。
---

# FlaUI Desktop BDD 測試撰寫標準

以 `demo2_desktop_app/Automation_testcase/Project_FlaUIBDD/` 為參考實作（Semi Inspection Desktop，10 TC 全通過）。

## 必讀路徑

| 用途 | 路徑（相對 `demo2_desktop_app/`） |
|------|-----------------------------------|
| 測試計畫 | `Automation_testcase/Test_cases/TEST_PLAN.md` |
| 案例規格表 | `Automation_testcase/Test_cases/SemiInspection_10_TestCases.md` |
| Feature | `Automation_testcase/Project_FlaUIBDD/Testcase_demo2_desktop_FlaUI_BDD/Features/*.feature` |
| Steps | `.../StepDefinitions/*Steps.cs` |
| Page Objects | `.../PageObjects/` |
| Hooks / 設定 | `.../Hooks/TestHooks.cs`、`App.config` |
| 詳細模板 | [reference.md](reference.md) |

## 三層文件（新增 TC 必同步）

1. **TEST_PLAN.md** — 目的、步驟、預期、Defect#
2. **SemiInspection_10_TestCases.md**（或同專案 `*_10_TestCases.md`）— 總表 + 控制項對照
3. **Feature + Steps + Page** — 可執行 BDD

**規則：** 一個 Scenario = 一個 TC；標題格式 `TCxx - 簡短描述`；標籤 `@Functional` / `@Negative` + 領域標籤（`@Import`、`@RawData` 等）。

## 專案結構（固定）

```
Project_FlaUIBDD/Testcase_*_FlaUI_BDD/
├── Features/*.feature
├── StepDefinitions/*Steps.cs
├── PageObjects/
│   ├── BasePage.cs
│   ├── MainWindowPage.cs      # 工具列、快捷鍵、About
│   ├── WorkspacePage.cs       # treeFiles、dataGrid、日誌/status
│   ├── FileDialogPage.cs      # #32770 / 1148 / 鍵盤 fallback
│   └── MessageBoxPage.cs
├── Hooks/TestHooks.cs
├── Helpers/ConfigHelper.cs, TestDataHelper.cs, DialogHelper.cs
├── App.config
└── specflow.json
```

## 撰寫流程（Agent 依序）

```
- [ ] 1. 讀 TEST_PLAN + 控制項表，確認 AutomationId / 快捷鍵
- [ ] 2. 在 .feature 新增 Scenario（繁體 Gherkin）
- [ ] 3. 重用既有 Step；不足才新增 StepDefinitions
- [ ] 4. UI 操作放 Page Object，Steps 只做編排與 Assert
- [ ] 5. 更新 App.config 路徑（EXE、Recipe_data）— 勿硬編在 Step
- [ ] 6. build_semi.bat → dotnet build → dotnet test -c Release
- [ ] 7. 更新 TEST_PLAN / 案例表 / 操作說明.html（若行為變更）
```

## Gherkin 規範

- **語言：** 繁體中文步驟；Feature 標題可中英混合。
- **Given：** `測試資料已就緒`、`應用程式已啟動`、`應用程式已重新啟動`（需乾淨狀態時）。
- **When：** `我點擊工具列「RawData」` — 用**使用者可見名稱**，非 AutomationId。
- **Then：** 斷言檔案、可見性、日誌關鍵字；MessageBox 關閉用 `我關閉訊息對話框`（When/Then 皆需 binding）。

### Scenario 模板

```gherkin
@Functional @RawData
Scenario: TC03 - RawData 參數表
  Given 測試資料已就緒
  And 應用程式已啟動
  When 我點擊工具列「RawData」
  Then 資料表應可見
  And 日誌區應包含「RawData」
```

## Page Object 規範

| 類別 | 職責 | 禁止 |
|------|------|------|
| `MainWindowPage` | 工具列、Ctrl 快捷鍵、About 選單鍵盤 | 在 Step 直接 FindElement |
| `WorkspacePage` | treeFiles、dataGridParameters、LogContains | 在 Page 寫 Assert |
| `FileDialogPage` | 開啟檔案對話框 | 假設只有 desktop 第一層 Window |
| `MessageBoxPage` | 確定/OK | 只認 Demo2 舊標題 |
| `BasePage` | FindWinFormsControl、ReadText、Click | — |

### WinForms 定位優先序

1. `AutomationId` = Designer `Name`（如 `btnParameters`、`treeFiles`）
2. `ByName` 顯示文字（RawData、Import Recipe）
3. `ControlType` fallback（Tree、DataGrid、Edit）

### 操作優先序（實戰）

| 情境 | 做法 |
|------|------|
| 工具列 ToolStrip | **Ctrl 快捷鍵優先**；Import Recipe 用 Ctrl+I |
| ToolStrip Invoke 逾時 | 改快捷鍵或 F10 選單（About：F10→Tools→About） |
| 開啟檔案對話框 | 搜尋 `#32770`、AutomationId `1148`、主視窗子樹；找不到則鍵盤 Alt+D 輸入路徑 |
| 讀取 txtToolLog | UIA Value 常失敗 → `WM_GETTEXT` + 掃描 descendant；並讀 `statusLabel` |
| 無可點擊點 | `BoundingRectangle` 中心 `Mouse.Click` |

**被測 App 配合（建議）：** 關鍵操作更新 `statusLabel.Text` 含英文關鍵字（如 `RawData:`、`Import Recipe:`），供自動化穩定斷言。

## StepDefinitions 規範

- 一個 `[Binding]` 類別；用 `ScenarioContext` 取 Page。
- 工具列步驟用 regex：`我點擊工具列「(.*)」`。
- Import / Run Inspection 在 Step 層可轉快捷鍵，邏輯仍委託 `MainWindowPage`。
- 斷言訊息含控制項名稱，方便對照 Inspector。

## Hooks 規範

- `[BeforeScenario]`：LaunchApplication → 等主視窗 → 注入 Page（含 `FileDialogPage(automation, mainWindow)`）。
- `[AfterScenario]`：失敗截圖 → `CloseApplication`（kill process，避免殘留）。
- `RelaunchApplication()`：TC01、TC08 等需初始狀態時使用。

## App.config 鍵（必設）

```xml
<add key="ApplicationPath" value="...SemiInspectionDesktop.exe" />
<add key="ApplicationTitle" value="Semi Inspection Desktop" />
<add key="ProcessName" value="SemiInspectionDesktop" />
<add key="RecipeDataDirectory" value="...Recipe_data" />
<add key="SampleRecipe" value="InspectionRecipe_Sample.json" />
<add key="DefaultTimeout" value="30000" />
<add key="TakeScreenshotOnFailure" value="true" />
```

## 建置與執行

```bat
cd demo2_desktop_app
build_semi.bat
cd Automation_testcase\Project_FlaUIBDD\Testcase_demo2_desktop_FlaUI_BDD
dotnet build -c Release
dotnet test -c Release
```

報告：`bin/Release/net8.0-windows/reports/SemiInspectionTestReport.html`  
截圖：`bin/Release/net8.0-windows/Screenshots/`

## 控制項對照（Semi Inspection 現行）

| 顯示名稱 | AutomationId | 快捷鍵 |
|----------|--------------|--------|
| Import Recipe | btnImportRecipe | Ctrl+I |
| Run Inspection | btnRunInspection | Ctrl+R |
| RawData | btnParameters | Ctrl+E |
| Defect Chart | btnDefectChart | Ctrl+D |
| About | btnToolbar0About | Tools→About |
| 參數表 | dataGridParameters | — |
| 檔案樹 | treeFiles | — |
| 日誌 | txtToolLog | — |
| 狀態列 | statusLabel | — |

## 與 TestComplete 轉換 skill 的關係

- **TestComplete → FlaUI 轉換：** 用 `Project_Testcomplete/skill.md`
- **本 skill：** 轉換完成後或從零撰寫/維護 FlaUI BDD 的**標準與實作細節**

## 完成檢查清單

- [ ] TC 編號與 TEST_PLAN Defect# 一致
- [ ] Feature Scenario 標題含 TCxx
- [ ] 無硬編 exe/資料路徑（走 ConfigHelper）
- [ ] 新 Step 有對應 Page 方法
- [ ] 負向案例有 MessageBox / 不當機斷言
- [ ] `dotnet test -c Release` 全綠
- [ ] 未提交 secrets（.env、token）

## 更多範例

見 [reference.md](reference.md)：FileDialog 實作、LogContains、Feature 全文、常見失敗排除。
