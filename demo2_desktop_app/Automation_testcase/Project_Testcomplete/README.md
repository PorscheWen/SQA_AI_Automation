# Project_Testcomplete — Semi Inspection Desktop

TestComplete Python 測試腳本，對應 [Test_cases/SemiInspection_10_TestCases.md](../Test_cases/SemiInspection_10_TestCases.md)。

## 前置條件

1. 建置：`demo2_desktop_app\build_semi.bat`
2. 確認路徑：`SemiInspectionDesktop\bin\Debug\SemiInspectionDesktop.exe`
3. `Recipe_data\InspectionRecipe_Sample.json` 存在

## 控制項 Name Mapping

| 功能 | AutomationId |
|------|--------------|
| Import Recipe | btnImportRecipe |
| RawData | btnParameters |
| Defect Chart | btnDefectChart |
| 參數表 | dataGridParameters |
| 檔案樹 | treeFiles |

## 參考

FlaUI BDD 專案：[../Project_FlaUIBDD](../Project_FlaUIBDD)
