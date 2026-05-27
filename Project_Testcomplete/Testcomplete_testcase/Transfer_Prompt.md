# TestComplete to FlaUI BDD 轉換 Prompt 範本

> 📖 **詳細說明請參考**: [Transfer_Prompt_Readme.md](Transfer_Prompt_Readme.md)

---

## 1️⃣ Feature File 轉換 Prompt

```
請將以下 TestComplete Python 測試案例轉換為 SpecFlow Feature file (.feature)：

**要求：**
- 使用 Gherkin 語法 (Given, When, Then)
- 每個 testcase_ 函數轉換為一個 Scenario
- 測試步驟使用自然語言描述
- 包含 Feature 標題和描述
- 使用繁體中文或英文（根據需求）
- 加入 @tags 標籤分類測試案例

**輸入程式碼：**
[貼上 TestComplete Python 測試案例]
```

---

## 2️⃣ Step Definitions 轉換 Prompt

```
請將以下 control_ 函數轉換為 SpecFlow Step Definitions (C#)：

**要求：**
- 使用 FlaUI 語法取代 TestComplete API
- 實作 [Given], [When], [Then] 屬性
- 使用 ScenarioContext 共享資料
- 處理元素查找和驗證
- 加入錯誤處理和截圖機制
- 使用 AutomationId, Name, ClassName 等定位策略

**輸入程式碼：**
[貼上 control_ 函數]
```

---

## 3️⃣ Page Object Model 轉換 Prompt

```
請將 control_ 函數群組轉換為 Page Object Model 類別：

**要求：**
- 每個頁面/元件建立獨立的 Page Object 類別
- 封裝元素定位器 (Locators)
- 封裝操作方法 (Actions)
- 使用 FlaUI API
- 實作 IDisposable 介面進行資源管理

**輸入程式碼：**
[貼上 control_ 函數群組]
```

---

## 4️⃣ 測試基礎架構轉換 Prompt

```
請建立 FlaUI BDD 測試專案的基礎架構：

**要求：**
- Hooks 類別 (BeforeScenario, AfterScenario)
- BasePage 基礎類別
- 應用程式啟動和關閉邏輯
- 截圖和日誌記錄機制
- 配置檔案管理

**專案資訊：**
- 應用程式名稱: [填入]
- 應用程式路徑: [填入]
- 測試框架: NUnit / MSTest
```

---

## 5️⃣ 元素定位策略轉換 Prompt

```
請將 TestComplete 的 XPath/TestId 定位轉換為 FlaUI 定位策略：

**對照表：**
- FindChildByXPath("//*[@data-testid='xxx']") → ByAutomationId("xxx")
- FindChild("ObjectIdentifier", "value") → ByName("value")
- WndClass → ByClassName("className")
- contentText → element.Name 或 element.Text

**輸入程式碼：**
[貼上包含元素定位的程式碼]
```

---

## 6️⃣ 斷言轉換 Prompt

```
請將 TestComplete 的驗證邏輯轉換為 NUnit/MSTest 斷言：

**要求：**
- 使用 Assert 類別
- 加入有意義的錯誤訊息
- 支援軟斷言 (繼續執行後續驗證)

**輸入程式碼：**
[貼上驗證相關的 control_ 函數]
```

---

## 7️⃣ 完整轉換 Prompt

```
請將以下完整的 TestComplete 測試案例轉換為 FlaUI BDD 結構：

**輸入：**
[貼上完整的 Testcase_shopping_cart 檔案內容]

**輸出需求：**
1. ShoppingCart.feature (所有測試場景)
2. ShoppingCartSteps.cs (步驟定義)
3. ShoppingCartPage.cs (Page Object)
4. ProductListPage.cs (Page Object)
5. TestHooks.cs (測試 Hooks)
6. BasePage.cs (基礎類別)

**技術要求：**
- SpecFlow 3.x+
- FlaUI 4.x+
- NUnit 3.x+ 或 MSTest
- .NET 6.0+
- 支援平行執行測試
- 包含錯誤處理和重試機制
```

---

## 📋 快速參考

### TestComplete → FlaUI 對照

| 功能 | TestComplete | FlaUI |
|------|-------------|-------|
| 尋找元素 | `FindChildByXPath()` | `FindFirstDescendant(cf => cf.ByAutomationId())` |
| 點擊 | `element.Click()` | `element.Click()` |
| 取得文字 | `element.contentText` | `element.Name` 或 `element.Text` |
| 等待 | `page.Wait()` | `element.WaitUntilResponsive()` |
| 驗證 | `Log.Error()` | `Assert.AreEqual()` |

### 使用步驟

1. 選擇適合的 Prompt (1-7)
2. 複製 Prompt 內容
3. 填入你的 TestComplete 程式碼
4. 提交給 AI 進行轉換
5. 檢查並調整輸出結果

---

📖 **完整範例與說明**: [Transfer_Prompt_Readme.md](Transfer_Prompt_Readme.md)
