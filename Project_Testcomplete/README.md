# Project TestComplete

TestComplete Python 測試腳本與轉換指南。測試結果報告依共用 **[report_prompt.md](report_prompt.md)**（TestComplete Summary Report 格式）產生。

---

## 專案結構

```
Project_Testcomplete/
├── report_prompt.md          # 報告格式與 Prompt 規格（共用）
├── generate_report.ps1       # 從 JUnit 產生 Summary Report
├── reports/                  # 測試報告輸出目錄
│   ├── junit-results.xml     # TestComplete 匯出的 JUnit（需自行產生）
│   ├── summary_report.md
│   └── summary_report.html
└── Testcomplete_testcase/
    ├── Testcase_shopping_cart
    ├── Transfer_Prompt.md
    └── Transfer_Prompt_Readme.md
```

---

## 產生測試報告（Summary Report）

### 步驟 1：在 TestComplete 執行測試並匯出 JUnit

任選一種方式，將 JUnit 存到 `reports/junit-results.xml`：

**方式 A — Summary Report 工具列**

1. 執行 `Testcase_shopping_cart` 測試
2. 開啟 **Summary Report**
3. 點 **Export Summary as JUnit**
4. 儲存為 `Project_Testcomplete/reports/junit-results.xml`

**方式 B — 腳本結尾（建議加入主程式）**

```python
Log.SaveResultsAs(
    r"C:\path\to\SQA_AI_Automation\Project_Testcomplete\reports\junit-results.xml",
    lsJUnit
)
```

**方式 C — 命令列**

```text
TestComplete.exe YourProject.pjs /r /ExportSummary:"...\reports\junit-results.xml"
```

### 步驟 2：產生報告

```powershell
cd Project_Testcomplete
.\generate_report.ps1
```

輸出：

| 檔案 | 說明 |
|------|------|
| `reports/summary_report.md` | Markdown 摘要（report_prompt 結構） |
| `reports/summary_report.html` | 瀏覽器可開啟的 HTML 摘要 |

---

## 測試案例

| ID | 函數 | 說明 |
|----|------|------|
| TC01 | `testcase_add_single_item` | 加入單一商品 |
| TC02 | `testcase_add_multiple_items` | 加入多種商品 |
| TC03 | `testcase_increase_quantity` | 增加數量 |
| TC04 | `testcase_remove_item` | 移除商品 |
| TC05 | `testcase_clear_cart` | 清空購物車 |
| TC06 | `testcase_checkout` | 結帳流程 |

腳本位置：[Testcomplete_testcase/Testcase_shopping_cart](Testcomplete_testcase/Testcase_shopping_cart)

---

## 相關文件

- [report_prompt.md](report_prompt.md) — Summary Report 格式規格
- [Transfer_Prompt.md](Testcomplete_testcase/Transfer_Prompt.md) — 轉 FlaUI BDD 的 Prompt
- [Project_FlaUIBDD](../Project_FlaUIBDD/README.md) — 轉換後的 FlaUI 專案（同格式報告）
