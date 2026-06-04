# Project_Testcomplete — Demo2 Desktop App

TestComplete Python 測試腳本，對應 [Test_cases/Demo2_10_TestCases.md](../Test_cases/Demo2_10_TestCases.md)。

## 檔案

| 檔案 | 說明 |
|------|------|
| `Testcomplete_testcase/Testcase_demo2_desktop.md` | 10 個 testcase 函數（TC01–TC10） |
| `../Test_cases/Demo2_10_TestCases.md` | 測試案例規格來源 |

## 測試案例對照

| ID | 類別 | 函數 |
|----|------|------|
| TC01 | Functional | `testcase_tc01_import_excel` |
| TC02 | Functional | `testcase_tc02_file_tree` |
| TC03 | Functional | `testcase_tc03_data_table` |
| TC04 | Functional | `testcase_tc04_draw_chart` |
| TC05 | Functional | `testcase_tc05_file_tree_open_excel` |
| TC06 | Functional | `testcase_tc06_about` |
| TC07 | Negative | `testcase_tc07_invalid_import` |
| TC08 | Negative | `testcase_tc08_chart_no_data` |
| TC09 | Negative | `testcase_tc09_missing_file` |
| TC10 | Compatibility | `testcase_tc10_excel_format_compat` |

## 前置條件

1. 建置 Demo2：`demo2_desktop_app\build.bat`
2. 確認路徑存在：`Demo2Desktop\bin\Debug\Demo2Desktop.exe`
3. TestComplete 已安裝並可執行 Desktop 測試
4. `Test_data\X.xlsx` 等樣本檔存在

## 執行

在 TestComplete 中：

1. 建立 **Script** 專案或 General 專案
2. 匯入 `Testcomplete_testcase/Testcase_demo2_desktop.md` 為 Script
3. 執行 `TestMain()` 或個別 `testcase_tc**`

```python
# 單一案例
testcase_tc03_data_table()

# 全部案例
control_run_all_tests()
```

## Name Mapping 建議

首次執行建議對主視窗建立 Alias，以提升工具列 / TreeView / DataGridView 穩定度。腳本已含 `WndCaption`、`ClrClassName` 後備搜尋。

## TestComplete → FlaUI

使用 Agent Skill：[skill.md](skill.md)（詳見 [Transfer_Prompt_Readme.md](Transfer_Prompt_Readme.md)）。

已轉換專案：[../Project_FlaUIBDD/README.md](../Project_FlaUIBDD/README.md)

## 參考

格式參考 demo1：`demo1_shoppingcart_website/Automation_testcase/Project_Testcomplete/Testcomplete_testcase/Testcase_shopping_cart.md`
