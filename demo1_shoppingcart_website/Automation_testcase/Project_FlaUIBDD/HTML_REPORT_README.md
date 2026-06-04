# FlaUI BDD HTML 測試報告功能說明

## 📊 概述

本專案已整合 ExtentReports 框架,可在測試執行時自動生成美觀的 HTML 格式測試報告。

## ✨ 功能特色

- ✅ **自動生成**: 測試執行時自動生成 HTML 報告,無需額外步驟
- 📸 **截圖整合**: 測試失敗時自動截圖並嵌入報告
- 📝 **詳細記錄**: 記錄每個測試步驟的執行結果
- 🎨 **美觀界面**: 採用現代化設計,易於閱讀
- 📊 **統計資訊**: 顯示測試通過率、總數等統計數據
- 🏷️ **標籤分類**: 支援測試場景標籤和分類

## 🚀 快速開始

### 方法一: 使用便捷腳本(推薦)

```powershell
# 執行測試並生成 HTML 報告
.\run-tests-html.ps1

# 執行測試並自動開啟報告
.\run-tests-html.ps1 -OpenReport

# 指定測試篩選
.\run-tests-html.ps1 -Filter "Name~購物車"

# 使用 Release 配置
.\run-tests-html.ps1 -Configuration Release -OpenReport
```

### 方法二: 使用 dotnet 命令

```powershell
cd Testcase_shopping_cart_FlaUI_BDD

# 還原套件(首次執行)
dotnet restore

# 建置專案
dotnet build

# 執行測試(會自動生成 HTML 報告)
dotnet test
```

## 📁 報告文件位置

測試完成後,報告文件會自動生成在以下位置:

```
Project_FlaUIBDD/
└── Testcase_shopping_cart_FlaUI_BDD/
    └── reports/
        ├── TestReport.html        # ← HTML 測試報告(主要)
        ├── junit-results.xml      # JUnit XML 格式報告
        ├── template.html          # HTML 報告模板
        └── screenshots/           # 測試截圖目錄
            └── *.png              # 失敗測試的截圖
```

## 📋 報告內容說明

### 1. 報告標題區
- 顯示專案名稱和測試框架資訊

### 2. 測試摘要
顯示測試統計資訊:
- **總測試數**: 執行的測試總數
- **通過**: 成功通過的測試數
- **失敗**: 失敗的測試數
- **跳過**: 被跳過的測試數

### 3. 測試執行資訊
包含以下詳細資訊:
- 專案名稱
- 測試環境 (Windows)
- 測試框架 (FlaUI + SpecFlow)
- 執行時間
- 測試持續時間
- 通過率百分比

### 4. 測試結果明細
- 按 Feature 分組顯示測試場景
- 顯示每個 Scenario 的執行狀態
- 記錄每個測試步驟(Given/When/Then)的結果
- 失敗測試會顯示錯誤訊息和堆疊追蹤
- 嵌入失敗測試的截圖

## 🎨 報告外觀

報告採用現代化設計,包含:
- 漂亮的漸層色標題
- 不同狀態的顏色標記:
  - 🟢 **綠色**: 測試通過
  - 🔴 **紅色**: 測試失敗
  - 🟡 **黃色**: 測試跳過
- 可展開/收合的 Feature 區塊
- 響應式設計,適合不同螢幕尺寸

## ⚙️ 自訂設定

### 修改報告路徑或名稱

在 `Hooks/TestHooks.cs` 的 `BeforeTestRun` 方法中修改:

```csharp
HtmlReportHelper.InitializeReport("reports", "TestReport.html");
```

### 自訂報告樣式

修改 `Helpers/HtmlReportHelper.cs` 中的配置:

```csharp
htmlReporter.Config.Theme = Theme.Dark;  // 改為深色主題
htmlReporter.Config.DocumentTitle = "自訂標題";
htmlReporter.Config.ReportName = "自訂報告名稱";
```

### 修改 HTML 模板

可以編輯 `reports/template.html` 來自訂報告外觀。

## 🔧 整合到 CI/CD

HTML 報告可以整合到 CI/CD 流程中:

```yaml
# 範例: GitHub Actions
- name: Run Tests
  run: |
    cd Project_FlaUIBDD
    .\run-tests-html.ps1

- name: Upload Test Report
  uses: actions/upload-artifact@v3
  with:
    name: test-report
    path: Project_FlaUIBDD/Testcase_shopping_cart_FlaUI_BDD/reports/
```

## 📦 相關套件

本功能使用以下 NuGet 套件:

- **ExtentReports** (v5.0.2): HTML 報告生成引擎
- **SpecFlow.NUnit** (v3.9.74): BDD 測試框架
- **FlaUI.UIA3** (v4.0.0): UI 自動化框架
- **JunitXml.TestLogger** (v6.1.0): JUnit XML 格式支援

## 🐛 常見問題

### Q1: 報告沒有生成?

**A**: 確認以下事項:
1. 已執行 `dotnet restore` 還原 NuGet 套件
2. 測試已成功執行完畢
3. 檢查是否有權限寫入 reports 目錄

### Q2: 截圖沒有顯示在報告中?

**A**: 檢查 `App.config` 中的設定:
```xml
<add key="TakeScreenshotOnFailure" value="true" />
<add key="ScreenshotDirectory" value="reports/screenshots" />
```

### Q3: 想要使用深色主題?

**A**: 在 `HtmlReportHelper.cs` 中修改:
```csharp
htmlReporter.Config.Theme = Theme.Dark;
```

### Q4: 如何只生成特定測試的報告?

**A**: 使用測試篩選:
```powershell
.\run-tests-html.ps1 -Filter "Name~購物車"
```

## 📚 更多資訊

- [ExtentReports 官方文檔](http://extentreports.com/)
- [SpecFlow 文檔](https://docs.specflow.org/)
- [FlaUI 文檔](https://github.com/FlaUI/FlaUI)

## 📝 版本資訊

- **版本**: 1.0.0
- **最後更新**: 2026-05-28
- **維護者**: SQA Team

---

💡 **提示**: 使用 `-OpenReport` 參數可以在測試完成後自動在瀏覽器中開啟報告!
