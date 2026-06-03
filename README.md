# SQA_AI_Automation

自動化測試專案集合 — 包含 TestComplete Python、FlaUI BDD 和 Pytest BDD 三種測試框架的完整實作範例。

## 🎯 專案總覽

本專案包含三個完整的測試自動化專案：

| 專案 | 框架 | 平台 | 狀態 | 說明 |
|------|------|------|------|------|
| **TestComplete** | Python | Windows/Linux | ✅ 完成 | Web 測試（原始專案） |
| **FlaUI BDD** | C# + SpecFlow | Windows 限定 | ✅ 完成 | Windows 桌面應用測試 |
| **Pytest BDD** | Python + Selenium | 跨平台 | ✅ 完成 | Web 測試（BDD 風格） |

> 📖 **桌面應用程式轉換指南**：若要將 Desktop App TestComplete 專案遷移至 FlaUI BDD，請參閱 [桌面應用程式轉換工作流程](Project_FlaUIBDD/桌面應用程式轉換工作流程.md)，涵蓋環境建構、程式碼轉換、報告整合及 Bamboo CI/CD 完整流程。

---

## 📁 完整目錄結構

```
SQA_AI_Automation/
├── demo/
│   └── shopping_cart/              # 測試用購物車網頁
├── tools/
│   └── generate_summary_report.py  # 共用 Summary Report 產生器
├── Project_Testcomplete/           # TestComplete 測試專案
│   ├── report_prompt.md            # Summary Report 格式規格（共用）
│   ├── generate_report.ps1
│   └── Testcomplete_testcase/
│       ├── Testcase_shopping_cart  # Python 測試腳本
│       └── Transfer_Prompt.md      # 轉換指南
├── Project_FlaUIBDD/               # FlaUI BDD 測試專案 (Windows)
│   ├── run-tests-and-report.ps1
│   ├── generate_report.ps1
│   └── Testcase_shopping_cart_FlaUI_BDD/
├── Project_PytestBDD/              # Pytest BDD 測試專案 (跨平台) ⭐ 新增
│   ├── features/                   # Gherkin 測試場景
│   ├── step_definitions/           # 步驟定義
│   ├── page_objects/               # Page Object Model
│   └── README.md
└── README.md                       # 本文件
```

---

## 🐍 Pytest BDD 專案（新增）⭐

### 專案位置
[Project_PytestBDD/](Project_PytestBDD/)

### 特色
- ✅ **跨平台** - Linux/Windows/macOS 皆可執行
- ✅ **BDD 風格** - Gherkin 中文測試場景
- ✅ **Selenium** - 支援多種瀏覽器
- ✅ **Page Object Model** - 清晰的程式碼架構
- ✅ **自動報告** - HTML 報告和失敗截圖
- ✅ **9 個測試場景** - 涵蓋購物車所有功能

### 快速執行
```bash
cd Project_PytestBDD
pip install -r requirements.txt

# 啟動測試網頁（另開終端）
cd ../demo/shopping_cart
python serve.py

# 執行測試
cd ../../Project_PytestBDD
pytest --headless
```

### 詳細文件
- [完整 README](Project_PytestBDD/README.md)
- [快速啟動](Project_PytestBDD/QUICK_START.md)
- [執行狀況](Project_PytestBDD/STATUS_REPORT.md)

---

## 📊 三種框架比較

| 特性 | TestComplete | FlaUI BDD | Pytest BDD |
|------|-------------|-----------|------------|
| 語言 | Python | C# | Python |
| 測試對象 | Web 應用 | Windows 應用 | Web 應用 |
| 平台 | 跨平台 | Windows 限定 | 跨平台 |
| BDD 支援 | ❌ | ✅ SpecFlow | ✅ Pytest-BDD |
| 學習曲線 | 中 | 高 | 低 |
| 成本 | 商業軟體 | 免費 | 免費 |
| 適用場景 | 商業自動化 | Windows 桌面測試 | 開源 Web 測試 |

---

## 🚀 TestComplete Python 專案（原始）

TestComplete Python 網頁測試案例 — 使用 TestComplete Python script 建立一般網頁 UI 自動化測試的指南與範例。

## 前置準備

| 項目 | 說明 |
|------|------|
| TestComplete | 已安裝，專案語言選 **Python** |
| 瀏覽器 | Chrome / Edge / Firefox（建議固定一種） |
| 被測網站 | URL 可正常開啟 |
| 測試帳號 | 若有登入流程，先準備測試資料 |

在 TestComplete 建立 **Web Testing** 專案，並確認 Python 腳本引擎可用。

## 建立測試案例的兩種方式

### 方式 A：錄製後改寫（最快）

1. **Record New Test** → 選 Web Testing
2. 手動操作：開網頁、點按鈕、輸入、驗證
3. 停止錄製 → TestComplete 產生 Python 腳本
4. 把重複步驟抽成函式，去掉硬編碼

### 方式 B：手寫 Python（較穩定）

直接寫 script，搭配 **Name Mapping** 管理元素，後續 UI 小改動較好維護。

## 典型腳本結構

- `TestMain()` 是 TestComplete 預設入口函式
- 用 **Aliases**（Name Mapping）而不是寫死 XPath
- 每個業務步驟拆成一個函式（open、login、verify 分開）

```python
def TestMain():
    open_browser("https://example.com/login")
    login("testuser", "password123")
    verify_homepage_title("Dashboard - Example")
    Aliases.browser.Close()
```

## Name Mapping（元素對應）

TestComplete 會把 DOM 元素對應成可讀名稱：

```
Aliases.browser
  └── pageLogin
        ├── textboxUsername
        ├── textboxPassword
        └── buttonSubmit
```

好處：

- UI 小改時只改 Mapping，不用改全部 script
- 腳本可讀性高，接近自然語言步驟

錄製時會自動建立；手寫時可在 **Name Mapping** 面板手動新增。

## 一般網頁常用操作

| 操作 | Python 範例 |
|------|-------------|
| 開啟網址 | `Browsers.Item[btChrome].Run(url)` |
| 等待頁面 | `Sys.Browser("*").WaitPage("*", 30000)` |
| 輸入文字 | `Aliases.browser.page.inputField.SetText("text")` |
| 點擊 | `Aliases.browser.page.buttonSubmit.Click()` |
| 下拉選單 | `Aliases.browser.page.comboBox.ClickItem("選項")` |
| 驗證文字 | `aqObject.CheckProperty(Aliases.browser.page.label, "contentText", cmpEqual, "預期文字")` |
| 截圖 | `Sys.Browser("*").Page("*").Picture().SaveToFile("screenshot.png")` |
| 關閉瀏覽器 | `Aliases.browser.Close()` |

## 組成 Test Case

在 TestComplete 專案樹：

```
Project
├── Script
│   └── Unit1.py          ← Python 腳本
├── NameMapping
│   └── Aliases           ← 元素對應
└── TestItems
    ├── LoginTest         → 執行 login + verify
    ├── SearchTest        → 執行 search + verify
    └── CheckoutTest      → 執行 checkout flow
```

每個 **Test Item** 可指向單一 `TestMain()` 或不同 Python routine。

## 資料驅動（多組測試資料）

用 **TestComplete DDT** 或 CSV / Excel：

```python
def TestMain():
    for row in DDT.CurrentDriver:
        username = row["Username"]
        password = row["Password"]
        expected = row["ExpectedTitle"]

        open_browser("https://example.com/login")
        login(username, password)
        verify_homepage_title(expected)
        Aliases.browser.Close()
```

一個 script 可跑多組登入、搜尋、表單等案例。

## 建議的測試案例清單（一般網站）

1. **導航**：首頁、選單、麵包屑
2. **表單**：必填、格式錯誤、成功提交
3. **登入 / 登出**：成功、失敗、Session
4. **搜尋**：有結果、無結果、特殊字元
5. **列表 / 分頁**：翻頁、排序、篩選
6. **Modal / Popup**：開啟、關閉、確認
7. **響應式**（可選）：不同視窗大小

## 實務建議

1. **先錄 1 條 happy path**，再重構成函式
2. **一個函式只做一件事**
3. **等待用 Wait / WaitProperty**，少用固定 `Delay`
4. **驗證要明確**（文字、URL、元素可見性）
5. **失敗時截圖**，方便 debug
6. 案例變多後，可再考慮轉 **BDD / Gherkin**

## 如何使用本目錄範例

1. 在 TestComplete 建立 Web Testing 專案（Python）
2. 將 `shopping_website/` 內腳本複製到專案的 Script 單元
3. 依實際網站調整 URL、Name Mapping 與驗證條件
4. 在 Test Items 新增項目，指向 `TestMain()` 執行

### 執行最小範例

- 腳本：`examples/example_static_page.py`
- 目標：`https://example.com`
- 驗證：頁面 `<h1>` 文字為 `Example Domain`

### 執行購物車完整測試套件（推薦）

1. 啟動示範頁：

```powershell
cd demo/shopping_cart
python serve.py
```

2. 瀏覽器開啟 `http://localhost:8888/` 確認頁面正常。

3. 在 TestComplete 匯入 `shopping_website/web_shopping_cart_test_suite.py`，執行 `TestMain()`。

詳細測試計劃見 `shopping_website/TEST_PLAN.md`（11 案例：功能 6 + 負面 3 + 相容 2）。

## 後續：轉換為 BDD

若需將 Python script 轉為 BDD（Gherkin `.feature`）：

1. 盤點現有 Test Items 與 script 函式
2. 將每個業務步驟對應為 Given / When / Then
3. 建立 step definitions 呼叫原有 Python 函式
4. 保留 Name Mapping，僅改測試描述層

---

## 🎯 FlaUI BDD 專案（已實現）

本專案已將 TestComplete 購物車測試轉換為 **FlaUI + SpecFlow BDD** 測試：

### 📂 位置
```
Project_FlaUIBDD/
└── Testcase_shopping_cart_FlaUI_BDD/   # 購物車 BDD 測試專案
```

### ✨ 特色
- ✅ 使用 **SpecFlow 3.9** (BDD 框架)
- ✅ 使用 **FlaUI 4.0** (Windows UI Automation)
- ✅ 使用 **NUnit 4.3** (測試執行器)
- ✅ 7 個 Gherkin 測試場景
- ✅ 完整 Page Object Model 架構
- ✅ 中文測試場景描述

### ⚠️ 環境需求
**必須在 Windows 環境執行**（需要 Windows UI Automation API）

- Windows 10/11
- .NET 8.0 SDK
- Python 3.x（測試網頁伺服器）

### 📖 相關文件
- [完整專案說明](Project_FlaUIBDD/README.md)
- [Windows 執行指南](Project_FlaUIBDD/HOW_TO_RUN_ON_WINDOWS.md)
- [轉換提示指南](Project_Testcomplete/Testcomplete_testcase/Transfer_Prompt.md)

### 🚀 快速執行（Windows）
```powershell
# 啟動測試網頁
cd demo\shopping_cart
python serve.py

# 執行 BDD 測試（另開視窗）
cd Project_FlaUIBDD\Testcase_shopping_cart_FlaUI_BDD
dotnet test
```

---

## 參考

- [TestComplete Documentation](https://support.smartbear.com/testcomplete/docs/)
- [Web Testing in TestComplete](https://support.smartbear.com/testcomplete/docs/testing-with/testcomplete/web-testing/)
- [FlaUI GitHub](https://github.com/FlaUI/FlaUI)
- [SpecFlow Documentation](https://docs.specflow.org/)
