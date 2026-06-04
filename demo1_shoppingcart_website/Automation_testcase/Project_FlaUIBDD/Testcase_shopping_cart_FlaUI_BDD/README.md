# Shopping Cart FlaUI BDD 測試案例

本專案是從 TestComplete Python 測試案例轉換為 FlaUI C# BDD 測試結構的**完整獨立專案**。

## ✅ 專案狀態

- ✅ **可獨立建置和執行** - 包含完整的 .csproj 配置
- ✅ **所有 NuGet 套件已安裝** - SpecFlow 3.9.74, FlaUI 4.0.0, NUnit 4.3.2
- ✅ **建置成功** - 0 錯誤，僅有編碼風格警告
- ✅ **7 個測試場景** - 包含參數化測試
- ✅ **完整的 Page Object Model** - 3 個頁面類別，1 個基礎類別
- ✅ **測試基礎架構完整** - Hooks、Helpers、Configuration

---

## � 快速開始

### 1. 建置專案
```bash
cd Testcase_shopping_cart_FlaUI_BDD
dotnet restore
dotnet build
```

### 2. 配置應用程式路徑
編輯 `App.config`:
```xml
<add key="ApplicationPath" value="你的應用程式路徑" />
<add key="ApplicationUrl" value="http://localhost:8080" />
```

### 3. 執行測試（需要 Windows 環境）
```bash
dotnet test
```

**注意**：FlaUI 測試必須在 Windows 環境執行。目前在 Linux 環境可以編輯和建置，但無法執行實際測試。

---

## �📋 轉換來源

- **原始測試案例**: `Testcase_shopping_cart` (TestComplete Python)
- **轉換依據**: [Transfer_Prompt.md](../../Testcomplete_testcase/Testcomplete_testcase/Transfer_Prompt.md)
- **轉換日期**: 2026-05-27

---

## 🏗️ 專案結構

```
Testcase_shopping_cart_FlaUI_BDD/
├── Features/
│   └── ShoppingCart.feature          # Gherkin 測試場景（7個場景）
├── StepDefinitions/
│   └── ShoppingCartSteps.cs          # SpecFlow 步驟定義
├── PageObjects/
│   ├── BasePage.cs                   # 基礎頁面類別
│   ├── ProductListPage.cs            # 產品列表頁面
│   └── ShoppingCartPage.cs           # 購物車頁面
├── Hooks/
│   └── TestHooks.cs                  # 測試 Hooks (Before/After)
├── Helpers/
│   ├── ScreenshotHelper.cs           # 截圖輔助工具
│   └── ConfigHelper.cs               # 配置管理工具
├── App.config                         # 應用程式配置
└── README.md                          # 本文件
```

---

## 📝 測試場景清單

### 原始 TestComplete 測試案例 → FlaUI BDD 場景

| ID | TestComplete 函數 | BDD Scenario | 標籤 |
|----|------------------|--------------|------|
| TC01 | `testcase_add_single_item()` | 加入單一商品 | @AddItem |
| TC02 | `testcase_add_multiple_items()` | 加入多種商品 | @AddItem @Multiple |
| TC03 | `testcase_increase_quantity()` | 使用加號按鈕增加數量 | @Quantity |
| TC04 | `testcase_remove_item()` | 移除商品後購物車回到空狀態 | @RemoveItem |
| TC05 | `testcase_clear_cart()` | 加入多項後清空購物車 | @ClearCart |
| TC06 | `testcase_checkout()` | 結帳流程驗證 | @Checkout |
| NEW | - | 驗證不同商品的加入功能 (參數化) | @Parametrized |

---

## 🔄 轉換對照表

### Control 函數 → Page Object 方法

| TestComplete | FlaUI BDD |
|-------------|-----------|
| `control_element("click", test_id="add-apple")` | `ProductListPage.ClickAddApple()` |
| `control_element("get_text", test_id="cart-count")` | `ShoppingCartPage.GetCartCount()` |
| `control_element("verify_text", ...)` | `Assert.AreEqual(expected, actual)` |
| `control_reset_cart()` | `ShoppingCartPage.ResetCart()` |
| `control_click_testid(...)` | `BasePage.ClickElement(...)` |

### 元素定位策略

| TestComplete | FlaUI |
|-------------|-------|
| `FindChildByXPath("//*[@data-testid='xxx']")` | `FindElement("xxx")` (AutomationId) |
| `element.contentText` | `element.Name` 或 `element.Text` |
| `element.Click()` | `element.AsButton().Click()` |
| `Log.Message()` | `TestContext.WriteLine()` |
| `Log.Error()` | `Assert.AreEqual()` 失敗訊息 |

---

## 🚀 執行測試

### 前置需求

1. **.NET 8.0+** 已安裝
2. **Windows 作業系統** (FlaUI 需要)
3. **購物車應用程式** 已部署並可存取

### 配置應用程式路徑

編輯 `App.config` 或設定環境變數：

```xml
<add key="ApplicationPath" value="C:\Path\To\ShoppingCart\index.html" />
```

或

```bash
set SHOPPING_CART_APP_PATH=C:\Path\To\ShoppingCart\index.html
```

### 執行所有測試

```bash
dotnet test
```

### 執行特定標籤的測試

```bash
# 只執行購物車測試
dotnet test --filter "TestCategory=ShoppingCart"

# 只執行加入商品測試
dotnet test --filter "TestCategory=AddItem"

# 執行結帳測試
dotnet test --filter "TestCategory=Checkout"
```

### 產生詳細報告

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 📊 測試案例詳細說明

### TC01: 加入單一商品

**Gherkin:**
```gherkin
Scenario: TC01 - 加入單一商品
  When 我點擊加入蘋果按鈕
  Then 購物車件數應該是 "1 件"
  And 購物車總計應該是 "NT$ 30"
  And 蘋果數量應該是 "1"
```

**對應 TestComplete:**
```python
def testcase_add_single_item():
    control_reset_cart()
    control_click_testid("add-apple")
    control_verify_text("cart-count", "1 件", "Cart count")
    control_verify_text("cart-total", "NT$ 30", "Cart total")
```

### TC02: 加入多種商品

**Gherkin:**
```gherkin
Scenario: TC02 - 加入多種商品
  When 我點擊加入蘋果按鈕
  And 我點擊加入香蕉按鈕
  And 我點擊加入牛奶按鈕
  Then 購物車件數應該是 "3 件"
  And 購物車總計應該是 "NT$ 105"
```

**對應 TestComplete:**
```python
def testcase_add_multiple_items():
    control_click_testid("add-apple")
    control_click_testid("add-banana")
    control_click_testid("add-milk")
    control_verify_text("cart-count", "3 件", "Cart count")
```

---

## 🎯 Page Object Model 設計

### ProductListPage

管理產品列表相關操作：
- `ClickAddApple()` - 加入蘋果
- `ClickAddBanana()` - 加入香蕉
- `ClickAddMilk()` - 加入牛奶
- `AddItem(itemName)` - 動態加入商品

### ShoppingCartPage

管理購物車相關操作：
- `GetCartCount()` - 取得購物車件數
- `GetCartTotal()` - 取得購物車總計
- `ResetCart()` - 重置購物車
- `ClickIncreaseQuantity(item)` - 增加商品數量
- `ClickRemoveItem(item)` - 移除商品
- `ClickCheckout()` - 結帳
- 各種 `Verify...()` 驗證方法

### BasePage

所有頁面的基礎類別：
- `FindElement(automationId)` - 尋找元素
- `ClickElement(automationId)` - 點擊元素
- `GetElementText(automationId)` - 取得文字
- `VerifyElementText(...)` - 驗證文字
- `TakeScreenshot(path)` - 截圖

---

## 🔧 配置選項

### App.config 參數

| 參數 | 說明 | 預設值 |
|------|------|--------|
| ApplicationPath | 應用程式路徑 | C:\Path\To\ShoppingCart\index.html |
| ApplicationUrl | 應用程式 URL | http://localhost:8080 |
| BrowserType | 瀏覽器類型 | Chrome |
| DefaultTimeout | 預設超時時間（毫秒） | 10000 |
| TakeScreenshotOnFailure | 失敗時截圖 | true |
| ScreenshotDirectory | 截圖目錄 | Screenshots |

### 環境變數（優先於 App.config）

```bash
set ApplicationPath=C:\Your\App\Path
set DefaultTimeout=15000
set TakeScreenshotOnFailure=true
```

---

## 📸 截圖功能

測試失敗時會自動截圖，儲存位置：
```
bin/Debug/net8.0-windows/Screenshots/
```

檔案命名格式：
```
{ScenarioTitle}_{yyyyMMdd_HHmmss}.png
```

---

## ⚠️ 注意事項

### 1. Windows 專用
FlaUI 只能在 Windows 環境執行，因為它使用 Windows UI Automation API。

### 2. 元素定位策略
本專案使用 **AutomationId** 作為主要定位策略，對應 TestComplete 的 `data-testid`：

```csharp
// TestComplete: data-testid="add-apple"
// FlaUI: AutomationId="add-apple"
FindElement("add-apple")
```

### 3. 應用程式類型
目前支援：
- Windows 應用程式 (.exe)
- Web 應用程式 (需配合瀏覽器)

### 4. 同步機制
使用 `WaitUntilResponsive()` 和 `Retry` 機制確保元素已載入。

---

## 🐛 疑難排解

### 問題 1: 找不到元素

**原因**: AutomationId 不正確或元素尚未載入

**解決方案**:
1. 使用 FlaUI Inspect 工具檢查 AutomationId
2. 增加 timeout 時間
3. 檢查元素是否在正確的視窗層級

### 問題 2: 應用程式無法啟動

**原因**: 路徑設定錯誤或權限不足

**解決方案**:
1. 檢查 `App.config` 中的 ApplicationPath
2. 確認檔案存在且有執行權限
3. 查看 TestHooks 中的啟動邏輯

### 問題 3: 測試執行很慢

**原因**: 預設 timeout 太長

**解決方案**:
1. 調整 App.config 中的 DefaultTimeout
2. 為特定元素設定較短的 timeout
3. 使用更精確的元素定位策略

---

## 📚 相關資源

### 原始專案文件
- [Testcase_shopping_cart](../../Testcomplete_testcase/Testcomplete_testcase/Testcase_shopping_cart) - 原始 TestComplete 測試案例
- [Transfer_Prompt.md](../../Testcomplete_testcase/Testcomplete_testcase/Transfer_Prompt.md) - 轉換 Prompt 範本
- [Transfer_Prompt_Readme.md](../../Testcomplete_testcase/Testcomplete_testcase/Transfer_Prompt_Readme.md) - 詳細轉換指南

### 官方文件
- [FlaUI GitHub](https://github.com/FlaUI/FlaUI)
- [SpecFlow 文件](https://docs.specflow.org/)
- [NUnit 文件](https://docs.nunit.org/)
- [Gherkin 語法](https://cucumber.io/docs/gherkin/)

### 工具
- [FlaUI Inspect](https://github.com/FlaUI/FlaUIInspect) - UI 元素檢查工具
- [SpecFlow for Visual Studio](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio) - VS 擴充套件

---

## 🎉 轉換完成

此專案已成功將 6 個 TestComplete Python 測試案例轉換為完整的 FlaUI BDD 結構，包含：

✅ 7 個 Gherkin 測試場景（包含參數化測試）  
✅ Page Object Model 架構  
✅ SpecFlow Step Definitions  
✅ 測試 Hooks (BeforeScenario/AfterScenario)  
✅ 截圖和日誌功能  
✅ 配置管理  
✅ 錯誤處理機制  

現在可以在 Windows 環境執行完整的 UI 自動化測試！🚀
