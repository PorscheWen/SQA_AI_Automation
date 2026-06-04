# TestComplete 到 FlaUI BDD 轉換檢查清單

> 📋 使用此清單確保轉換過程不遺漏任何重要步驟

---

## 📂 準備階段

- [ ] **備份原始專案**
  - 建立 Git 分支或完整備份
  - 確保可以隨時回滾到原始狀態

- [ ] **環境準備**
  - [ ] 安裝 Visual Studio (Community/Professional/Enterprise)
  - [ ] 安裝 SpecFlow 擴展
  - [ ] 安裝 FlaUI NuGet 套件
  - [ ] 確認 .NET Framework/Core 版本

- [ ] **分析現有測試**
  - [ ] 列出所有 testcase_ 函數 (共 ___ 個)
  - [ ] 列出所有 control_ 函數 (共 ___ 個)
  - [ ] 識別可重用的函數群組
  - [ ] 記錄測試相依性和執行順序

---

## 🎯 第一階段：Feature File 轉換

### 測試案例轉換

- [ ] **TC01 - 加入單一商品**
  - [ ] 撰寫 Feature 描述
  - [ ] 轉換為 Gherkin Scenario
  - [ ] 加入適當的 @tags
  - [ ] 驗證步驟邏輯正確

- [ ] **TC02 - 加入多種商品**
  - [ ] 撰寫 Scenario
  - [ ] 加入 @tags
  - [ ] 驗證步驟

- [ ] **TC03 - 增加數量**
  - [ ] 撰寫 Scenario
  - [ ] 加入 @tags
  - [ ] 驗證步驟

- [ ] **TC04 - 移除商品**
  - [ ] 撰寫 Scenario
  - [ ] 加入 @tags
  - [ ] 驗證步驟

- [ ] **TC05 - 清空購物車**
  - [ ] 撰寫 Scenario
  - [ ] 加入 @tags
  - [ ] 驗證步驟

- [ ] **TC06 - 結帳流程**
  - [ ] 撰寫 Scenario
  - [ ] 加入 @tags
  - [ ] 驗證步驟

### Feature File 品質檢查

- [ ] 使用清晰的 Given-When-Then 結構
- [ ] 步驟使用自然語言描述
- [ ] 避免技術實作細節
- [ ] @tags 分類合理（功能、優先級等）
- [ ] 包含 Feature 層級的描述

---

## 🏗️ 第二階段：Page Object 建立

### BasePage 基礎類別

- [ ] **建立 BasePage.cs**
  - [ ] 實作 FindElement 方法
  - [ ] 實作 ClickElement 方法
  - [ ] 實作 GetElementText 方法
  - [ ] 實作 WaitForElement 方法
  - [ ] 加入錯誤處理
  - [ ] 實作 IDisposable 介面
  - [ ] 加入 XML 文檔註解

### ProductListPage

- [ ] **建立 ProductListPage.cs**
  - [ ] 定義元素定位器常數
    - [ ] AddAppleButtonId
    - [ ] AddBananaButtonId
    - [ ] AddMilkButtonId
  - [ ] 實作操作方法
    - [ ] ClickAddApple()
    - [ ] ClickAddBanana()
    - [ ] ClickAddMilk()
    - [ ] AddItem(string itemName)
    - [ ] AddMultipleItems(params string[] items)
  - [ ] 繼承自 BasePage
  - [ ] 加入 XML 文檔註解

### ShoppingCartPage

- [ ] **建立 ShoppingCartPage.cs**
  - [ ] 定義元素定位器常數
    - [ ] CartCountId
    - [ ] CartTotalId
    - [ ] ResetCartButtonId
    - [ ] CheckoutButtonId
  - [ ] 實作取得資訊方法
    - [ ] GetCartCount()
    - [ ] GetCartTotal()
    - [ ] GetItemQuantity(string itemName)
  - [ ] 實作操作方法
    - [ ] ClickIncreaseQuantity(string itemName)
    - [ ] ClickDecreaseQuantity(string itemName)
    - [ ] ClickRemoveItem(string itemName)
    - [ ] ResetCart()
    - [ ] ClickCheckout()
    - [ ] CloseCheckoutModal()
  - [ ] 繼承自 BasePage
  - [ ] 加入 XML 文檔註解

### Page Object 品質檢查

- [ ] 所有 Page Object 都繼承自 BasePage
- [ ] 元素定位器使用常數定義
- [ ] 方法名稱清晰明確
- [ ] 適當的存取修飾詞（public/private/protected）
- [ ] 完整的 XML 文檔註解
- [ ] 沒有 UI 驗證邏輯（驗證應在 Step Definitions）

---

## 📝 第三階段：Step Definitions 實作

### Given 步驟

- [ ] **Given購物車已清空**
  - [ ] 呼叫 ShoppingCartPage.ResetCart()
  - [ ] 加入錯誤處理
  - [ ] 加入日誌輸出

- [ ] **Given我已開啟購物網站**
  - [ ] 驗證應用程式狀態
  - [ ] 加入錯誤處理

### When 步驟 - 加入商品

- [ ] **When我點擊加入{商品}按鈕**
  - [ ] 支援蘋果、香蕉、牛奶
  - [ ] 使用正則表達式參數化
  - [ ] 加入錯誤處理和截圖

### When 步驟 - 數量調整

- [ ] **When我點擊{商品}的增加數量按鈕**
  - [ ] 實作增加邏輯
  - [ ] 加入錯誤處理

- [ ] **When我點擊{商品}的減少數量按鈕**
  - [ ] 實作減少邏輯
  - [ ] 加入錯誤處理

- [ ] **When我點擊移除{商品}按鈕**
  - [ ] 實作移除邏輯
  - [ ] 加入錯誤處理

### When 步驟 - 購物車操作

- [ ] **When我點擊清空購物車按鈕**
  - [ ] 實作清空邏輯
  - [ ] 加入錯誤處理

- [ ] **When我點擊結帳按鈕**
  - [ ] 實作結帳邏輯
  - [ ] 加入錯誤處理

### Then 步驟 - 驗證

- [ ] **Then購物車件數應該是{數量}**
  - [ ] 實作驗證邏輯
  - [ ] 使用 NUnit Assert
  - [ ] 失敗時截圖

- [ ] **Then購物車總計應該是{金額}**
  - [ ] 實作驗證邏輯
  - [ ] 使用 NUnit Assert
  - [ ] 失敗時截圖

- [ ] **Then{商品}的數量應該是{數量}**
  - [ ] 實作驗證邏輯
  - [ ] 使用 NUnit Assert
  - [ ] 失敗時截圖

### Step Definitions 品質檢查

- [ ] 所有步驟都已實作
- [ ] 使用適當的 Binding 屬性 ([Given], [When], [Then])
- [ ] 正則表達式正確匹配 Feature 步驟
- [ ] 建構函數注入 ScenarioContext
- [ ] 從 ScenarioContext 取得 Page Objects
- [ ] 完整的錯誤處理（try-catch）
- [ ] 失敗時自動截圖
- [ ] 適當的日誌輸出（TestContext.WriteLine）

---

## 🔧 第四階段：Hooks 和輔助類別

### TestHooks

- [ ] **建立 TestHooks.cs**
  - [ ] [BeforeScenario] - 測試前初始化
    - [ ] 建立 UIA3Automation 實例
    - [ ] 啟動應用程式
    - [ ] 取得主視窗
    - [ ] 建立 Page Objects
    - [ ] 儲存到 ScenarioContext
  - [ ] [AfterScenario] - 測試後清理
    - [ ] 關閉應用程式
    - [ ] 釋放 Automation 資源
    - [ ] 測試失敗時截圖

### Helper 類別

- [ ] **ScreenshotHelper.cs**
  - [ ] TakeScreenshot() 方法
  - [ ] 自動命名（時間戳 + Scenario 名稱）
  - [ ] 儲存到 reports/screenshots 目錄

- [ ] **ConfigHelper.cs**
  - [ ] 讀取 App.config 設定
  - [ ] GetAppPath() 方法
  - [ ] GetTimeout() 方法

- [ ] **DemoServerHelper.cs**（如需要）
  - [ ] 啟動測試伺服器
  - [ ] 檢查伺服器狀態
  - [ ] 停止伺服器

---

## ✅ 第五階段：測試與驗證

### 單元測試每個 Page Object

- [ ] **ProductListPage 測試**
  - [ ] 驗證每個 Add 方法可正常執行
  - [ ] 驗證元素定位正確

- [ ] **ShoppingCartPage 測試**
  - [ ] 驗證所有取得方法
  - [ ] 驗證所有操作方法

### 執行 Scenario 測試

- [ ] **TC01 - 加入單一商品**
  - [ ] 測試通過 ✅
  - [ ] 失敗時有截圖
  - [ ] 日誌完整

- [ ] **TC02 - 加入多種商品**
  - [ ] 測試通過 ✅

- [ ] **TC03 - 增加數量**
  - [ ] 測試通過 ✅

- [ ] **TC04 - 移除商品**
  - [ ] 測試通過 ✅

- [ ] **TC05 - 清空購物車**
  - [ ] 測試通過 ✅

- [ ] **TC06 - 結帳流程**
  - [ ] 測試通過 ✅

### 整合測試

- [ ] 執行所有測試（Run All）
- [ ] 檢查測試報告
- [ ] 確認截圖功能正常
- [ ] 驗證日誌輸出完整

---

## 📊 第六階段：報告與文檔

### 測試報告

- [ ] **設定報告輸出**
  - [ ] 配置 SpecFlow+ LivingDoc（可選）
  - [ ] 或使用 Extent Reports
  - [ ] 或使用 Allure

- [ ] **驗證報告內容**
  - [ ] 測試結果統計正確
  - [ ] 失敗測試有截圖
  - [ ] 執行時間記錄

### 文檔更新

- [ ] **更新 README.md**
  - [ ] 專案架構說明
  - [ ] 安裝步驟
  - [ ] 執行方法
  - [ ] 常見問題

- [ ] **建立 API 對照文檔**
  - [ ] TestComplete → FlaUI 映射表
  - [ ] 常用操作範例

---

## 🚀 第七階段：優化與改進

### 程式碼品質

- [ ] **執行靜態分析**
  - [ ] 使用 SonarLint 或 ReSharper
  - [ ] 修復所有警告

- [ ] **程式碼審查**
  - [ ] 命名規範一致
  - [ ] 註解完整
  - [ ] 無重複程式碼

### 效能優化

- [ ] 減少不必要的等待時間
- [ ] 優化元素查找策略
- [ ] 並行執行可獨立的測試（如適用）

### CI/CD 整合

- [ ] 建立 Azure DevOps Pipeline（可選）
- [ ] 或建立 GitHub Actions workflow
- [ ] 設定自動化測試觸發條件
- [ ] 配置測試報告發布

---

## 📋 最終檢查清單

- [ ] **功能完整性**
  - [ ] 所有原始測試案例都已轉換
  - [ ] 測試覆蓋率與原專案相同或更好
  - [ ] 所有測試都能獨立執行

- [ ] **程式碼品質**
  - [ ] 無編譯錯誤
  - [ ] 無編譯警告（或已評估並接受）
  - [ ] 程式碼遵循團隊規範
  - [ ] 註解和文檔完整

- [ ] **測試穩定性**
  - [ ] 執行 10 次全部測試，成功率 ≥ 90%
  - [ ] 失敗的測試有明確原因（非隨機失敗）
  - [ ] 元素等待機制健全

- [ ] **維護性**
  - [ ] Page Object 結構清晰
  - [ ] Step Definitions 易於理解
  - [ ] 新增測試案例容易
  - [ ] 文檔齊全，新成員可快速上手

- [ ] **團隊準備**
  - [ ] 團隊成員已培訓
  - [ ] 建立知識庫和 FAQ
  - [ ] 定義支援流程

---

## 💡 使用建議

1. **列印此清單**：在轉換過程中，列印此清單並在完成每項時勾選
2. **團隊協作**：將清單拆分給不同成員，定期同步進度
3. **持續更新**：根據專案需求調整此清單
4. **記錄問題**：在清單旁記錄遇到的問題和解決方案

---

## 🔗 相關資源

- 📄 [Transfer_Prompt.md](Testcomplete_testcase/Transfer_Prompt.md) - AI 轉換 Prompt 範本
- 📖 [Transfer_Prompt_Readme.md](Testcomplete_testcase/Transfer_Prompt_Readme.md) - 詳細轉換指南
- 🌐 [使用說明書.html](使用說明書.html) - 完整專案文檔
- 🤖 [AI 工具章節](使用說明書.html#ai-tools) - AI 輔助轉換指南

---

**版本：** 1.0  
**更新日期：** 2026-06-01  
**維護者：** SQA_AI_Automation Team
