# Demo2 測試案例（Gherkin）

本文件為 **Demo2 Desktop App** 之 BDD 測試規格，採 **Gherkin** 語法（Feature / Scenario / Given-When-Then）。

| 項目 | 說明 |
|------|------|
| 對應自動化 | `Project_FlaUIBDD/.../Features/Demo2Desktop.feature` |
| 對應 TestComplete | `Project_Testcomplete/.../Testcase_demo2_desktop.md` |
| 測試資料 | `demo2_desktop_app/Test_data/`（含 `X.xlsx`） |
| 案例數 | 10（Functional 6、Negative 3、Compatibility 1） |

---

## 追溯對照（Metadata）

| ID | Tags | 類型 | 優先級 | Defect | 狀態 |
|----|------|------|--------|--------|------|
| TC01 | `@Functional @Import` | Import JSON | High | 2001 | Ready |
| TC02 | `@Functional @FileTree` | File Tree | High | 2002 | Ready |
| TC03 | `@Functional @DataTable` | Data Table | High | 2003 | Ready |
| TC04 | `@Functional @Chart` | Chart | High | 2004 | Ready |
| TC05 | `@Functional @FileTree` | File Tree Open | Medium | 2005 | Ready |
| TC06 | `@Functional @About` | About | Low | 2006 | Ready |
| TC07 | `@Negative` | Invalid Import | High | 2007 | Ready |
| TC08 | `@Negative @Chart` | Chart No Data | Medium | 2008 | Ready |
| TC09 | `@Negative` | Missing File | Medium | 2009 | Ready |
| TC10 | `@Compatibility` | Excel Format | High | 2010 | Ready |

---

## Gherkin 測試案例

```gherkin
# language: zh-TW
Feature: Demo2 桌面應用程式測試
  作為測試人員
  我想要驗證 Demo2 Desktop App 的檔案、資料表與圖表功能
  以確保 Test_data 工作流程正常

  # TC01 | Defect 2001 | High
  @Functional @Import
  Scenario: TC01 - Import JSON 至 Test_data
    Given Demo2Desktop 已啟動
    And Test_data 資料夾可寫入
    And 測試資料目錄存在樣本檔 "TestType_Defect.json"
    When 使用者點擊 Toolbar0「Import JSON」
    And 使用者在檔案對話框選擇有效檔案 "TestType_Defect.json"
    Then 檔案應複製至 Test_data 且檔名為 "TestType_Defect.json"
    And 工作區 Tab1 資料表應顯示該 JSON 內容
    And 左側 File Tree 應已更新
    And 日誌區應包含 "Import JSON"

  # TC02 | Defect 2002 | High
  @Functional @FileTree
  Scenario: TC02 - File Tree 顯示 Test_data
    Given 應用程式已啟動
    When 使用者檢視左側 File Tree
    And 使用者展開 Test_data 根節點
    Then 應顯示 Test_data 路徑下的檔案與子資料夾
    And 清單中應包含 "X.xlsx"

  # TC03 | Defect 2003 | High
  @Functional @DataTable
  Scenario: TC03 - Icon1 切換 Data Table
    Given 應用程式已啟動
    And Test_data 內存在 "X.xlsx"
    When 使用者點擊 Toolbar1「Data Table」
    Then 工作區應切換至 Tab1 資料表
    And 若尚未載入資料則應自動載入 "X.xlsx"
    And DataGrid 應顯示欄位與資料列
    And 日誌區應包含 "Data Table"

  # TC04 | Defect 2004 | High
  @Functional @Chart
  Scenario: TC04 - Icon2 繪製曲線圖
    Given 已載入含 Test Type 與 Defect Number 欄位的 Excel
    When 使用者點擊 Toolbar1「Draw data」
    Then 工作區應切換至 Tab2 圖表
    And 應顯示 X 軸為 Test Type、Y 軸為 Defect Number 的曲線圖
    And 圖表應包含資料點
    And 日誌區應包含 "Draw data"

  # TC05 | Defect 2005 | Medium
  @Functional @FileTree
  Scenario: TC05 - 檔案樹雙擊開啟 Excel
    Given Test_data 內存在 "X.xlsx" 或 "Demo2_10_TestCases.xlsx"
    When 使用者在 File Tree 雙擊該 xlsx 檔案
    Then Tab1 應顯示該檔第一個工作表內容
    And 狀態列或日誌應顯示檔案路徑
    And 日誌區應包含 "開啟"

  # TC06 | Defect 2006 | Low
  @Functional @About
  Scenario: TC06 - About 對話框
    Given 應用程式已啟動
    When 使用者點擊 Toolbar0「About」
    Then 應顯示 About 訊息對話框
    And 訊息應包含 File Tree 路徑說明
    And 訊息應包含工具列功能說明

  # TC07 | Defect 2007 | High
  @Negative
  Scenario: TC07 - 匯入非 Excel 檔
    Given 應用程式已啟動
    When 使用者點擊「Import Excel」
    And 使用者選擇非 Excel 檔案（如 .txt）
    Then 應顯示警告「請選擇 .xls / .xlsx / .xlsm」
    And 不應將無效檔寫入 Test_data

  # TC08 | Defect 2008 | Medium
  @Negative @Chart
  Scenario: TC08 - 無資料時繪圖
    Given 應用程式已啟動且 DataGrid 尚無資料
    When 使用者點擊「Draw data」
    Then 應提示找不到可繪圖資料或 Test_data\X.xlsx
    And 應用程式不應當機
    And 主視窗仍應可繼續操作

  # TC09 | Defect 2009 | Medium
  @Negative
  Scenario: TC09 - 開啟不存在檔案
    Given 應用程式已啟動
    When 使用者嘗試載入不存在的 xlsx 檔案
    Then 應顯示錯誤訊息與 OLEDB 相關提示
    And 應用程式仍應可繼續操作

  # TC10 | Defect 2010 | High
  @Compatibility
  Scenario: TC10 - xlsx 格式相容
    Given 已安裝 Excel 讀取元件（Jet 供 .xls、ACE 12.0 供 .xlsx）
    And Test_data 內存在 "X.xlsx"
    When 使用者開啟 "X.xlsx"
    Then DataGrid 應正常顯示第一個工作表
    And 欄位與資料列顯示應正確
    And 日誌區應包含 "Excel"
```

---

## 與自動化 Feature 對照

手動規格（上文）與 SpecFlow 實作步驟的對應關係：

| Scenario | FlaUI Step 摘要 |
|----------|-----------------|
| TC01 | `Given 測試資料已就緒` → Ctrl+I 匯入 → 選 `X.xlsx` → 驗證 Grid／日誌 |
| TC02 | 啟動 → 驗證標題、File Tree、`X.xlsx` 存在 |
| TC03 | 點「Data Table」→ 驗證 Grid、日誌 |
| TC04 | 先 Data Table → 點「Draw data」→ 驗證日誌 |
| TC05 | 檔案樹雙擊 xlsx → 驗證 Grid、日誌「開啟」 |
| TC06 | 點「About」→ 關閉對話框 |
| TC07 | 匯入 `.txt` → 驗證未寫入、關閉警告 |
| TC08 | 重啟後直接 Draw data → 主視窗仍存在 |
| TC09 | Ctrl+E + 不存在檔 → 主視窗仍存在 |
| TC10 | Data Table → 驗證 Grid、日誌含 Excel |

實作檔：`../Project_FlaUIBDD/Testcase_demo2_desktop_FlaUI_BDD/Features/Demo2Desktop.feature`

---

## 舊版表格規格（已取代）

原 Markdown 表格欄位（Test ID、Preconditions、Steps…）已整併至上方 Gherkin；追溯欄位見 **追溯對照** 表。
