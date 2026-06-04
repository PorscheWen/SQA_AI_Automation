# TestComplete Summary Report — 格式與內容 Prompt

> 依據官方文件整理：[Summary Report | TestComplete Documentation](https://support.smartbear.com/testcomplete/docs/testing-with/log/summary.html)  
> 適用版本：TestComplete 15.81（文件最後更新：2026-04-06）  
> 相關匯出說明：[Exporting Test Results](https://support.smartbear.com/testcomplete/docs/testing-with/log/working-with/exporting/index.html)、[Log.SaveResultsAs](https://support.smartbear.com/testcomplete/docs/reference/project-objects/test-log/log/saveresultsas.html)

---

## 用途

本文件供以下情境使用：

- 解讀 TestComplete 執行後的 **Summary Report（摘要報告）**
- 設計與 TestComplete 對齊的 **JUnit / HTML** 匯出或 CI 整合
- 作為 AI／文件撰寫的 **Prompt 範本**，產出相同結構的測試結果摘要

---

## 何時產生 Summary Report

符合**任一**條件，測試執行結束後 TestComplete 即產生 Summary Report：

| # | 觸發條件 | 說明 |
|---|----------|------|
| 1 | Execution Plan 含 Test Case | 專案套件或專案中，至少一項在 Execution Plan 標記為 test case |
| 2 | 依 Tag 執行 | 以 tag 或 tag expression 指定要跑的測試 |
| 3 | Script 手動標記 | 腳本內使用 `aqTestCase.Begin` 標記至少一個 test case |
| 4 | BDD | 執行內容包含 **BDD Feature** 或 **BDD Scenario**（不論是否標記為 test case） |

---

## Summary Report 內容結構（UI）

### 區塊一：執行摘要（Session Overview）

| 欄位／元素 | 說明 |
|------------|------|
| **Session 名稱** | 專案套件名、專案名、個別 test item 名、script routine、keyword test 名、tag 或 tag expression |
| **Run test cases** | 已執行的 test case 數量 |
| **Passed** | 通過數 |
| **Failed** | 失敗數 |
| **Warnings** | 含警告執行完成的數量 |
| **Rerun** | 重跑次數（含對 warning 或 passed 後重試的明細） |
| **Unexecuted** | 未開始執行的 test case（例如專案因錯誤中斷、模組不支援等） |
| **比例圖表** | 上述各狀態占比 |
| **Test run duration** | 整次測試總耗時 |
| **通過／失敗／警告統計圖** | 各狀態視覺化 |

### 區塊二：General（一般資訊）

| 欄位 | 說明 |
|------|------|
| Start Time | 測試執行開始時間 |
| End Time | 測試執行結束時間 |
| Computer | 執行測試的電腦名稱 |
| User | 執行測試的使用者帳號 |

### 區塊三：Test Case 清單（核心表格）

**列入規則：**

- 來源可為：Execution Plan 的 test case、`aqTestCase.Begin` 標記、BDD Feature／Scenario、依 tag 選取的測試
- 依**實際執行順序**排列（忽略 parent-child 階層）
- Execution Plan 中 **Count > 1** 時，每次 iteration 皆獨立列入

| 欄位名稱 | 說明 |
|----------|------|
| **Test Case** | 名稱來源：Execution Plan 的 Name 欄、`aqTestCase.Begin` 指定名、Code／Project Explorer 中的測試名（tag 執行時）、特殊格式 tag 名 |
| **Project** | 該 test case 所屬專案名稱 |
| **Start Time** | 該 test case 開始執行時間 |
| **Duration** | 該 test case 執行耗時 |

**互動行為：**

- 點表頭狀態（Passed / Failed / Warning 等）→ 篩選清單
- 點 **Test Case** 名稱 → 開啟詳細 log（有錯誤則跳到第一個錯誤，否則跳到 case 開頭）
- 若有**未標記為 test case** 的測試發生錯誤 → 顯示通知，可點擊進入詳細 log

---

## 匯出格式（Export）

### 工具列操作（Summary Report 畫面）

| 工具列項目 | 輸出格式 | 內容範圍 |
|------------|----------|----------|
| Export Summary as JUnit | JUnit 風格 XML | 僅 Summary（test case 層級統計） |
| Export full log to | MHT 或 HTML | Summary + 完整詳細 log |
| Send via E-Mail | MHT | Summary + 詳細 log，以郵件寄出 |
| Print | — | 列印 Summary |

### Log.SaveResultsAs 格式常數

| 常數 | 值 | 格式 | 說明 |
|------|-----|------|------|
| `lsXML` | 0 | XML | 原生 TestComplete log XML |
| `lsHTML` | 1 | HTML | 網頁 + 圖片、樣式等附檔（`index.htm`） |
| `lsMHT` | 2 | MHT | 單一網頁封存（IE 可開，Chrome/Firefox 不支援） |
| `lsZip` | 3 | ZIP | 內含 `.tcLog` 原生 log |
| `lsPackedHTML` | 4 | ZIP | 內含 HTML 格式 log |
| **`lsJUnit`** | **5** | **JUnit XML** | Summary 匯出為 JUnit 相容報告 |

### 程式與命令列範例

```javascript
// 匯出 Summary 為 JUnit XML
Log.SaveResultsAs("C:\\Work\\Log\\Summary.xml", lsJUnit);
```

```javascript
// 匯出完整 log 為 HTML
Log.SaveResultsAs("C:\\Work\\Log\\", lsHTML);
```

```text
/ExportSummary:"C:\Path\To\Summary.xml"
```

**注意：** `lsJUnit` 僅匯出 **Summary 層級**（各 test case 通過／失敗／時間），**不含**逐步操作細節。完整步驟請用 HTML/MHT 或原生 log。

---

## JUnit XML 結構（機器可讀格式）

TestComplete 的 JUnit 匯出與常見 JUnit 生態相容（JUnit Viewer、xUnit Viewer、Allure、CI parser 等）。

### 概念結構

```xml
<?xml version="1.0" encoding="UTF-8"?>
<testsuites tests="總數" failures="失敗數" errors="錯誤數" time="總秒數" timestamp="ISO8601">
  <testsuite name="Session或專案名" tests="..." failures="..." errors="..." skipped="..." time="...">
    <testcase name="Test Case 名稱" classname="Project 名稱" time="秒數">
      <!-- 失敗時可能包含 -->
      <failure message="..." type="...">...</failure>
      <!-- 或 -->
      <error message="..." type="...">...</error>
    </testcase>
    <!-- 依執行順序更多 testcase -->
  </testsuite>
</testsuites>
```

### 欄位對照（Summary UI → JUnit）

| Summary Report | JUnit XML（常見對應） |
|----------------|----------------------|
| Session / 專案 | `<testsuite name="...">` |
| Test Case | `<testcase name="...">` |
| Project | `classname` 屬性 |
| Duration | `time` 屬性（秒） |
| Failed | `<failure>` 子元素 |
| Error（執行異常） | `<error>` 子元素 |
| 整次執行統計 | `<testsuites>` 的 `tests`、`failures`、`errors`、`time` |

> 各解析工具對 `testsuites` 根節點、`properties`、`system-out` 等延伸欄位支援不一；整合 CI 前請用實際匯出檔驗證。

---

## 自動產生報告（兩專案共用）

共用工具：`SQA_AI_Automation/tools/generate_summary_report.py`  
格式規格：本文件 `report_prompt.md`

| 專案 | 一鍵指令 | JUnit 來源 | 報告輸出 |
|------|----------|------------|----------|
| **Project_Testcomplete** | `.\generate_report.ps1` | TestComplete 匯出 → `reports/junit-results.xml` | `reports/summary_report.{md,html}` |
| **Project_FlaUIBDD** | `.\run-tests-and-report.ps1` | `dotnet test` JUnit logger | `Testcase_shopping_cart_FlaUI_BDD/reports/summary_report.{md,html}` |

```powershell
# TestComplete：先匯出 JUnit，再產生報告
cd Project_Testcomplete
.\generate_report.ps1

# FlaUI BDD：執行測試並產生報告（Windows）
cd Project_FlaUIBDD
.\run-tests-and-report.ps1
```

手動呼叫工具：

```bash
python tools/generate_summary_report.py \
  --junit path/to/junit-results.xml \
  --output-dir path/to/reports \
  --session "SessionName" \
  --project "ProjectName" \
  --prompt-ref report_prompt.md
```

---

## 與 FlaUI BDD（NUnit）報告對照

本 repo 中 `Project_FlaUIBDD` 由 TestComplete 轉換而來，報告對照如下：

| TestComplete | FlaUI BDD / NUnit |
|--------------|-------------------|
| BDD Scenario → Summary 一列 | SpecFlow Scenario → NUnit test method |
| Summary 四欄表格 | `dotnet test` + TRX / HTML / JUnit logger |
| Export Summary as JUnit | `--logger "junit;LogFilePath=TestResults\junit-results.xml"` |

```powershell
cd Testcase_shopping_cart_FlaUI_BDD
dotnet test --logger "junit;LogFilePath=TestResults\junit-results.xml"
```

---

## Prompt 範本（供 AI 產出同結構報告）

將下列區塊複製到對話，並填入實際執行結果或 log 路徑。

### A. 解讀 TestComplete Summary Report

```
請依 TestComplete Summary Report 標準格式，解讀以下測試結果並產出繁體中文摘要：

**必須包含的區塊：**
1. 執行摘要：Session 名稱、Run/Passed/Failed/Warnings/Rerun/Unexecuted、總耗時
2. General：開始／結束時間、電腦、使用者（若有）
3. Test Case 清單表格：Test Case | Project | Start Time | Duration | 狀態
4. 失敗與警告項目的重點說明（含可點進詳細 log 的建議）
5. 若有未納入 Summary 的錯誤，單獨列出

**輸入（貼上 log 摘要、螢幕截圖文字或 JUnit XML）：**
[在此貼上內容]
```

### B. 產出 JUnit 相容摘要 XML

```
請將以下測試執行結果轉為 TestComplete Summary 相容的 JUnit XML（lsJUnit 風格）：

**規則：**
- 根元素使用 testsuites / testsuite / testcase
- 每個 test case 對應一筆 testcase，name=案例名，classname=專案名，time=秒數
- 失敗使用 failure，執行異常使用 error
- 保留執行順序

**輸入：**
[測試結果列表或表格]
```

### C. 撰寫測試結論報告（管理層可讀）

```
請將 TestComplete Summary Report 內容改寫為一份測試結論報告（繁體中文）：

**結構：**
1. 一句話結論（通過／未通過／需重測）
2. 執行範圍與環境（General 區塊資訊）
3. 數據摘要（Passed/Failed/Warnings 與比例）
4. 失敗案例清單與可能原因（依 Test Case 表格）
5. 建議後續動作（修復、重跑、阻擋上線等）

**輸入：**
[Summary 內容或 JUnit XML]
```

---

## 檢查清單（撰寫或驗證報告時）

- [ ] 是否說明 Session 名稱與執行範圍
- [ ] 是否列出 Run / Passed / Failed / Warnings / Unexecuted
- [ ] 是否包含總耗時與 General（時間、機器、使用者）
- [ ] Test Case 清單是否含四欄：Test Case、Project、Start Time、Duration
- [ ] 失敗項是否可追溯至詳細 log 或第一個錯誤點
- [ ] 匯出需求是否區分 Summary（JUnit）與 Full log（HTML/MHT）
- [ ] CI 整合是否使用 `lsJUnit` 或 `/ExportSummary`

---

## 參考連結

- [Summary Report](https://support.smartbear.com/testcomplete/docs/testing-with/log/summary.html)
- [Exporting Test Results](https://support.smartbear.com/testcomplete/docs/testing-with/log/working-with/exporting/index.html)
- [Log.SaveResultsAs Method](https://support.smartbear.com/testcomplete/docs/reference/project-objects/test-log/log/saveresultsas.html)
- [Tests, Test Items, and Test Cases](https://support.smartbear.com/testcomplete/docs/testing-with/test-items/index.html)（TestComplete 測試項目概念）

---

**最後更新：** 2026-05-28  
**檔案位置：** `SQA_AI_Automation/Project_Testcomplete/report_prompt.md`
