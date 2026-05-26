# SQA_AI_Automation

TestComplete Python 網頁測試案例 — 使用 TestComplete Python script 建立一般網頁 UI 自動化測試的指南與範例。

## 目錄結構

```
SQA_Transfer_Testcase/
├── README.md
├── demo/
│   └── shopping_cart/              # 購物車示範網頁（供 TestComplete 測試）
│       ├── index.html
│       ├── styles.css
│       ├── app.js
│       └── serve.py
├── shopping_website/               # TestComplete 測試腳本與測試計劃
│   ├── TEST_PLAN.md
│   ├── web_shopping_cart_test_suite.py
│   ├── web_shopping_cart_flow.py
│   ├── web_shopping_cart_aliases.py
│   └── web_login_flow.py
└── examples/
    └── example_static_page.py      # 最小可跑範例（靜態頁驗證）
```

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

## 參考

- [TestComplete Documentation](https://support.smartbear.com/testcomplete/docs/)
- [Web Testing in TestComplete](https://support.smartbear.com/testcomplete/docs/testing-with/testcomplete/web-testing/)
