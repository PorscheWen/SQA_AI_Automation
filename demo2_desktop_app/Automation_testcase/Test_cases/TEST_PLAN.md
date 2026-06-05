# Demo2 Desktop App — 測試計畫（10 Test Cases）

| 類別 | 數量 |
|------|------|
| Functional | 6 |
| Negative | 3 |
| Compatibility | 1 |
| **合計** | **10** |

資料檔：`Test_data/Demo2_10_TestCases.xlsx`（與 `Test_cases/Demo2_10_TestCases.xlsx` 相同內容）

---

## Functional Test（6）

### TC01 — Import JSON 至 Test_data
- **目的**：驗證 Toolbar0 Import JSON 可匯入並顯示資料。
- **步驟**：Import JSON → 選 .json → 確認 Tab1 與檔案樹。
- **預期**：檔案在 `Test_data`；DataGrid 有資料。
- **Defect#**：2001

### TC02 — File Tree 顯示 Test_data
- **目的**：左側樹狀目錄指向 `Test_data`。
- **步驟**：啟動程式 → 展開 File Tree。
- **預期**：列出 xlsx 等檔案。
- **Defect#**：2002

### TC03 — Icon1 Data Table
- **目的**：Data Table 按鈕切換並載入資料。
- **步驟**：點 Data Table。
- **預期**：Tab1 顯示 `X.xlsx` 或已載入表格。
- **Defect#**：2003

### TC04 — Icon2 Draw data 曲線圖
- **目的**：圖表 Tab 顯示 Test Type / Defect 曲線。
- **步驟**：點 Draw data。
- **預期**：Tab2 曲線圖（X=Test Type, Y=Defect）。
- **Defect#**：2004

### TC05 — 檔案樹雙擊開啟 Excel
- **目的**：從樹狀目錄直接開檔。
- **步驟**：雙擊 `Demo2_10_TestCases.xlsx` 或 `X.xlsx`。
- **預期**：Tab1 顯示工作表內容。
- **Defect#**：2005

### TC06 — About 對話框
- **目的**：About 顯示版本與路徑說明。
- **步驟**：點 Toolbar0 About。
- **預期**：MessageBox 含 File Tree 路徑。
- **Defect#**：2006

---

## Negative Test（3）

### TC07 — 匯入非 JSON 檔
- **步驟**：Import JSON → 選 .txt。
- **預期**：警告，不匯入。
- **Defect#**：2007

### TC08 — 無資料時繪圖
- **步驟**：無 DataGrid 資料 → Draw data。
- **預期**：提示無資料，程式不崩潰。
- **Defect#**：2008

### TC09 — 開啟不存在檔案
- **步驟**：載入已刪除/不存在路徑。
- **預期**：錯誤訊息 + OLEDB 說明。
- **Defect#**：2009

---

## Compatibility Test（1）

### TC10 — xls / xlsx 格式相容
- **步驟**：分別開啟 .xlsx 與 .xls（若環境已安裝 ACE/Jet）。
- **預期**：皆可讀取第一個工作表。
- **Defect#**：2010

---

## 執行方式

### 手動（Demo2 Desktop）

```bat
cd demo2_desktop_app
build.bat
run.bat
```

### TestComplete 自動化

腳本：`../Project_Testcomplete/Testcomplete_testcase/Testcase_demo2_desktop.md`  
執行 `TestMain()` 或個別 `testcase_tc01_import_excel` … `testcase_tc10_excel_format_compat`。

依 TC 編號在應用程式中手動執行，或將 Status 欄位更新於 `Demo2_10_TestCases.xlsx`。

## 重新產生 Excel

```bat
cd Test_cases
python generate_demo2_testcases.py
```
