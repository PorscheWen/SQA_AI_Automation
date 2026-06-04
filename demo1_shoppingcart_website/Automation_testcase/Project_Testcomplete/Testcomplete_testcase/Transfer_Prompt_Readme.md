# TestComplete to FlaUI BDD 轉換指南

> **測試報告**：兩專案皆依 [report_prompt.md](../report_prompt.md) 產生 Summary Report。TestComplete 執行後執行 `..\generate_report.ps1`；FlaUI 執行 `..\..\Project_FlaUIBDD\run-tests-and-report.ps1`。

## 📖 目錄
- [概述](#概述)
- [轉換目標](#轉換目標)
- [轉換範例](#轉換範例)
- [專案結構](#專案結構)
- [轉換檢查清單](#轉換檢查清單)
- [最佳實踐](#最佳實踐)
- [常見問題](#常見問題)

---

## 概述

本指南協助將 TestComplete Python 測試案例轉換為 FlaUI C# BDD 結構。

### 原始 TestComplete 結構說明
1. `testcase_` 為測試案例
2. `control_` 為測試控制項
3. `_aliases` 為測試控制項別名(Name mapping)

---

## 轉換目標

將 TestComplete Python 測試案例轉換為 FlaUI C# BDD 結構，使用：
- **SpecFlow** (BDD 框架)
- **Gherkin 語法** (Feature files)
- **Page Object Model** (POM 設計模式)
- **FlaUI** (UI 自動化框架)

---

## 轉換範例

### 1️⃣ Feature File 轉換範例

#### 輸入 (TestComplete Python):
```python
def testcase_add_single_item():
    """TC01：加入單一商品，驗證件數與總計。"""
    control_reset_cart()
    control_click_testid("add-apple")
    control_verify_text("cart-count", "1 件", "Cart count")
    control_verify_text("cart-total", "NT$ 30", "Cart total")
    control_verify_text("qty-value-apple", "1", "Apple quantity")
```

#### 輸出 (SpecFlow Feature):
```gherkin
Feature: 購物車功能測試
  作為一個使用者
  我想要測試購物車功能
  以確保商品管理運作正常

@ShoppingCart @AddItem
Scenario: TC01 - 加入單一商品
  Given 購物車已清空
  When 我點擊加入蘋果按鈕
  Then 購物車件數應該是 "1 件"
  And 購物車總計應該是 "NT$ 30"
  And 蘋果數量應該是 "1"
```

---

### 2️⃣ Step Definitions 轉換範例

#### 輸入 (TestComplete Python):
```python
def control_element(action, test_id=None, expected=None, label=None, timeout=10):
    if action == "click":
        element = control_element("find", test_id=test_id)
        element.Click()
    elif action == "verify_text":
        actual = control_element("get_text", test_id=test_id)
        return actual == expected
```

#### 輸出 (FlaUI C# Step Definitions):
```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using TechTalk.SpecFlow;
using NUnit.Framework;

[Binding]
public class ShoppingCartSteps
{
    private readonly UIA3Automation _automation;
    private Application _app;
    private Window _mainWindow;

    [Given(@"購物車已清空")]
    public void Given購物車已清空()
    {
        var cartCount = GetElementText("cart-count");
        if (cartCount != "0 件")
        {
            ClickElement("clear-cart");
        }
    }

    [When(@"我點擊加入(.*)按鈕")]
    public void When我點擊加入按鈕(string itemName)
    {
        var testId = $"add-{itemName.ToLower()}";
        ClickElement(testId);
    }

    [Then(@"購物車件數應該是 ""(.*)""")]
    public void Then購物車件數應該是(string expected)
    {
        var actual = GetElementText("cart-count");
        Assert.AreEqual(expected, actual, "購物車件數不符");
    }

    private void ClickElement(string automationId)
    {
        var element = FindElement(automationId);
        element?.Click();
    }

    private string GetElementText(string automationId)
    {
        var element = FindElement(automationId);
        return element?.Name ?? "";
    }

    private AutomationElement FindElement(string automationId)
    {
        return _mainWindow.FindFirstDescendant(cf => 
            cf.ByAutomationId(automationId))
            .AsButton();
    }
}
```

---

### 3️⃣ Page Object Model 轉換範例

#### 輸入 (TestComplete Python):
```python
def control_cart(action):
    if action == "reset":
        count_text = control_element("get_text", test_id="cart-count")
        if count_text != "0 件":
            control_element("click", test_id="clear-cart")
```

#### 輸出 (FlaUI C# Page Object):
```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;

public class ShoppingCartPage : BasePage
{
    // 元素定位器
    private const string CartCountId = "cart-count";
    private const string CartTotalId = "cart-total";
    private const string ClearCartId = "clear-cart";
    private const string CheckoutId = "checkout";

    public ShoppingCartPage(Window window) : base(window) { }

    // 元素屬性
    public AutomationElement CartCountElement => 
        FindElement(CartCountId);
    
    public AutomationElement CartTotalElement => 
        FindElement(CartTotalId);
    
    public Button ClearCartButton => 
        FindElement(ClearCartId).AsButton();

    // 操作方法
    public void ResetCart()
    {
        var count = GetCartCount();
        if (count != "0 件")
        {
            ClearCartButton.Click();
            WaitForElement(CartCountId);
        }
    }

    public string GetCartCount()
    {
        return CartCountElement?.Name ?? "";
    }

    public string GetCartTotal()
    {
        return CartTotalElement?.Name ?? "";
    }

    public void ClickCheckout()
    {
        FindElement(CheckoutId).AsButton().Click();
    }
}
```

---

### 4️⃣ 元素定位策略對照表

| TestComplete | FlaUI | 說明 |
|-------------|-------|------|
| `FindChildByXPath("//*[@data-testid='xxx']")` | `ByAutomationId("xxx")` | 使用 AutomationId |
| `FindChild("ObjectIdentifier", "value")` | `ByName("value")` | 使用元素名稱 |
| `WndClass` | `ByClassName("className")` | 使用 CSS 類別名稱 |
| `contentText` | `element.Name` 或 `element.Text` | 取得元素文字 |
| `Click()` | `Click()` | 點擊操作相同 |
| `Wait()` | `WaitUntilResponsive()` | 等待元素回應 |

#### 範例轉換：
```python
# TestComplete
xpath = "//*[@data-testid='%s']" % test_id
element = page.FindChildByXPath(xpath, timeout)
text = element.contentText
```

```csharp
// FlaUI
var element = window.FindFirstDescendant(cf => 
    cf.ByAutomationId(testId));
var text = element.Name;
```

---

### 5️⃣ 斷言轉換範例

#### 輸入 (TestComplete Python):
```python
def control_verify_text(test_id, expected, label):
    actual = control_get_text_by_testid(test_id)
    Log.Message("%s: expected=%s, actual=%s" % (label, expected, actual))
    if actual != expected:
        Log.Error("%s mismatch" % label)
        return False
    return True
```

#### 輸出 (FlaUI C# + NUnit):
```csharp
public void VerifyText(string automationId, string expected, string label)
{
    var actual = GetElementText(automationId);
    TestContext.WriteLine($"{label}: expected={expected}, actual={actual}");
    Assert.AreEqual(expected, actual, 
        $"{label} 不符: 預期 '{expected}', 實際 '{actual}'");
}
```

---

## 專案結構

### FlaUI BDD 測試專案結構：
```
FlaUIBDDTests/
├── Features/                      # Gherkin feature files
│   ├── ShoppingCart.feature
│   └── ProductList.feature
│
├── StepDefinitions/               # Step definitions
│   ├── ShoppingCartSteps.cs
│   └── ProductListSteps.cs
│
├── PageObjects/                   # Page Object Model
│   ├── BasePage.cs               # 基礎頁面類別
│   ├── ShoppingCartPage.cs       # 購物車頁面
│   └── ProductListPage.cs        # 產品列表頁面
│
├── Hooks/                         # Test hooks
│   └── TestHooks.cs              # BeforeScenario, AfterScenario
│
├── Helpers/                       # 輔助類別
│   ├── ScreenshotHelper.cs       # 截圖工具
│   ├── ConfigHelper.cs           # 配置管理
│   └── WaitHelper.cs             # 等待工具
│
├── Data/                          # 測試資料
│   └── TestData.json
│
└── App.config                     # 應用程式配置
```

---

## 轉換檢查清單

在轉換完成後，請確認以下項目：

### ✅ 功能完整性
- [ ] 所有 `testcase_` 函數已轉為 Scenario
- [ ] 所有 `control_` 函數已轉為 Step Definitions 或 Page Object 方法
- [ ] 測試邏輯保持一致
- [ ] 所有驗證點都已轉換

### ✅ 技術實作
- [ ] 使用正確的 FlaUI API 取代 TestComplete API
- [ ] 元素定位策略適用於目標應用程式
- [ ] 包含適當的等待和同步機制
- [ ] 加入錯誤處理和截圖機制

### ✅ 程式碼品質
- [ ] 測試可獨立執行（測試隔離）
- [ ] 使用有意義的變數和方法命名
- [ ] 加入適當的註解和文件
- [ ] 遵循 C# 編碼規範
- [ ] 遵循 SOLID 原則

### ✅ BDD 最佳實踐
- [ ] Scenario 使用自然語言描述
- [ ] Step Definitions 可重用
- [ ] 適當使用 Background 和 Scenario Outline
- [ ] 測試資料與邏輯分離

---

## 最佳實踐

### 1. 使用 Background
將共同的前置步驟提取到 Background 區塊：

```gherkin
Feature: 購物車功能測試

Background:
  Given 我已開啟購物網站
  And 購物車已清空

Scenario: 加入單一商品
  When 我點擊加入蘋果按鈕
  Then 購物車件數應該是 "1 件"
```

### 2. 使用 Scenario Outline
參數化測試以減少重複：

```gherkin
Scenario Outline: 加入不同商品
  When 我點擊加入<商品>按鈕
  Then 購物車件數應該是 "<件數>"
  And 購物車總計應該是 "<總計>"

Examples:
  | 商品 | 件數 | 總計     |
  | 蘋果 | 1 件 | NT$ 30  |
  | 香蕉 | 1 件 | NT$ 20  |
  | 牛奶 | 1 件 | NT$ 55  |
```

### 3. 單一職責原則
每個 Step Definition 只做一件事：

```csharp
// ✅ 好的做法
[When(@"我點擊加入蘋果按鈕")]
public void When我點擊加入蘋果按鈕()
{
    _productListPage.ClickAddApple();
}

// ❌ 避免的做法
[When(@"我點擊加入蘋果按鈕並驗證結果")]
public void When我點擊加入蘋果並驗證()
{
    _productListPage.ClickAddApple();
    Assert.AreEqual("1 件", _cartPage.GetCount()); // 驗證應在 Then 步驟
}
```

### 4. 可重用性
設計可在多個場景中重用的步驟：

```csharp
[When(@"我點擊加入(.*)按鈕")]
public void When我點擊加入按鈕(string itemName)
{
    _productListPage.ClickAddItem(itemName);
}
```

### 5. 清晰的命名
使用描述性的場景和步驟名稱：

```gherkin
# ✅ 好的命名
Scenario: 購物車為空時顯示提示訊息

# ❌ 避免的命名
Scenario: Test 1
```

---

## 常見問題

### Q1: 同步問題
**問題**: 測試執行過快，元素尚未載入

**解決方案**:
```csharp
// 使用 WaitUntilResponsive
element.WaitUntilResponsive();

// 或自訂等待條件
var retry = Retry.WhileException<ElementNotAvailableException>(
    () => element.Click(),
    timeout: TimeSpan.FromSeconds(10)
);
```

### Q2: 元素找不到
**問題**: FindElement 返回 null

**解決方案**:
```csharp
// 增加超時時間
var element = window.FindFirstDescendant(
    cf => cf.ByAutomationId(automationId),
    timeout: TimeSpan.FromSeconds(30)
);

// 或使用更穩定的定位器
var element = window.FindFirstDescendant(cf => 
    cf.ByAutomationId(automationId)
    .Or(cf.ByName(name))
);
```

### Q3: 應用程式未回應
**問題**: 應用程式偶爾卡住

**解決方案**:
```csharp
// 加入重試機制
public void ClickElementWithRetry(string automationId, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            var element = FindElement(automationId);
            element.Click();
            return;
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1) throw;
            Thread.Sleep(1000);
        }
    }
}
```

### Q4: 測試資料管理
**問題**: 硬編碼的測試資料難以維護

**解決方案**:
```gherkin
# 使用 SpecFlow Table
Scenario: 批次加入多個商品
  When 我加入以下商品:
    | 商品 | 數量 |
    | 蘋果 | 2    |
    | 香蕉 | 3    |
    | 牛奶 | 1    |
  Then 購物車總計應該正確

# 或使用 Examples
Scenario Outline: 驗證商品價格
  When 我查看<商品>的價格
  Then 價格應該是<價格>

Examples:
  | 商品 | 價格    |
  | 蘋果 | NT$ 30 |
  | 香蕉 | NT$ 20 |
```

---

## 技術要求

- **SpecFlow**: 3.x+
- **FlaUI**: 4.x+
- **NUnit / MSTest**: 3.x+
- **.NET**: 6.0+
- **C# 語言版本**: 10.0+

---

## 相關文件

- [Transfer_Prompt.md](Transfer_Prompt.md) - 轉換 Prompt 範本
- [Testcase_shopping_cart](Testcase_shopping_cart) - 原始 TestComplete 測試案例

---

## 參考資源

- [FlaUI Documentation](https://github.com/FlaUI/FlaUI)
- [SpecFlow Documentation](https://docs.specflow.org/)
- [Gherkin Syntax](https://cucumber.io/docs/gherkin/)
- [Page Object Model Pattern](https://martinfowler.com/bliki/PageObject.html)
