# Project FlaUI BDD

FlaUI BDD 測試專案集合，包含使用 SpecFlow + NUnit + FlaUI 框架的 UI 自動化測試。

---

## 📁 專案結構

```
Project_FlaUIBDD/
├── Testcase_shopping_cart_FlaUI_BDD/    # 購物車測試專案（完整獨立專案）
│   ├── Features/                         # Gherkin 測試場景
│   ├── StepDefinitions/                  # 步驟定義
│   ├── PageObjects/                      # Page Object Model
│   ├── Hooks/                            # 測試生命週期 Hooks
│   ├── Helpers/                          # 輔助工具類別
│   ├── reports/                          # JUnit + Summary Report 輸出
│   ├── App.config                        # 應用程式配置
│   ├── *.csproj                          # 專案檔案
│   └── README.md                         # 專案說明
├── web_dashboard/                       # 三 Tab 測試控制台網頁
│   ├── index.html                       # 三頁籤：勾選 / 進度 / 結果
│   ├── server.py                        # API 與 dotnet test 執行
│   └── static/                          # 前端資源
├── start-dashboard.ps1                  # 啟動控制台並開啟瀏覽器
├── run-tests-and-report.ps1             # 執行測試並產生報告（Windows）
├── generate_report.ps1                  # 僅從 JUnit 產生報告
├── HOW_TO_RUN_ON_WINDOWS.md             # Windows 執行指南
└── README.md                            # 本文件
```

---

## 🏗️ FlaUI 專案架構詳解

### 核心目錄結構

#### 📂 Features/ - 測試場景定義
存放 Gherkin 格式的 BDD 測試場景檔案。

| 檔案 | 說明 |
|------|------|
| `ShoppingCart.feature` | 購物車功能的 BDD 測試場景（Gherkin 語法） |
| `ShoppingCart.feature.cs` | SpecFlow 自動產生的程式碼檔案（不應手動編輯） |

**用途**: 
- 以 Given-When-Then 格式定義測試案例
- 支援參數化測試（Scenario Outline）
- 提供非技術人員可讀的測試文件

---

#### 📂 StepDefinitions/ - 步驟定義實作
實作 Feature 檔案中定義的測試步驟。

| 檔案 | 說明 |
|------|------|
| `ShoppingCartSteps.cs` | 購物車測試場景的步驟定義實作 |

**包含內容**:
- `[Given]` 步驟: 測試前置條件設定
- `[When]` 步驟: 執行使用者操作
- `[Then]` 步驟: 驗證測試結果
- ScenarioContext: 場景間資料共享

**範例方法**:
```csharp
[Given(@"使用者在產品列表頁面")]
[When(@"使用者將 ""(.*)"" 加入購物車")]
[Then(@"購物車中應該有 (.*) 項商品")]
```

---

#### 📂 PageObjects/ - 頁面物件模型
遵循 Page Object Model (POM) 設計模式，封裝頁面元素和操作。

| 檔案 | 說明 |
|------|------|
| `BasePage.cs` | 基礎頁面類別，提供所有頁面共用的方法 |
| `ProductListPage.cs` | 產品列表頁面的元素定位和操作方法 |
| `ShoppingCartPage.cs` | 購物車頁面的元素定位和驗證方法 |

**BasePage.cs 功能**:
- FlaUI Application 初始化
- 通用元素等待方法
- 視窗管理（最大化、關閉等）
- 錯誤處理和日誌記錄

**ProductListPage.cs 功能**:
- 定位產品元素（使用 AutomationId）
- 加入購物車按鈕點擊
- 購物車圖示互動
- 驗證產品可用性

**ShoppingCartPage.cs 功能**:
- 購物車項目計數驗證
- 產品名稱和價格驗證
- 數量調整（+/-按鈕）
- 移除商品功能
- 清空購物車操作
- 結帳流程驗證

---

#### 📂 Hooks/ - 測試生命週期鉤子
管理測試執行前後的設定和清理工作。

| 檔案 | 說明 |
|------|------|
| `TestHooks.cs` | SpecFlow 測試鉤子（Before/After Scenario/Step） |

**包含鉤子**:
- `[BeforeScenario]`: 場景執行前的初始化（啟動應用程式）
- `[AfterScenario]`: 場景執行後的清理（關閉應用程式、截圖）
- `[BeforeStep]` / `[AfterStep]`: 步驟級別的處理
- 失敗處理: 自動截圖和日誌記錄

---

#### 📂 Helpers/ - 輔助工具類別
提供跨專案使用的共用工具方法。

| 檔案 | 說明 |
|------|------|
| `ConfigHelper.cs` | 讀取 App.config 配置檔案（應用程式路徑等） |
| `ScreenshotHelper.cs` | 截圖功能（失敗時自動截圖） |
| `FailureLogHelper.cs` | 測試失敗日誌記錄和錯誤資訊收集 |

**ConfigHelper 功能**:
```csharp
GetAppPath()        // 取得待測應用程式路徑
GetTimeout()        // 取得元素等待逾時設定
GetScreenshotPath() // 取得截圖存放路徑
```

---

#### 📂 reports/ - 測試報告輸出
測試執行後產生的報告檔案存放目錄。

| 檔案 | 說明 |
|------|------|
| `junit-results.xml` | NUnit 產生的 JUnit 格式測試結果 |
| `summary_report.md` | Markdown 格式的測試摘要報告 |
| `summary_report.html` | HTML 格式的測試摘要報告（可瀏覽器開啟） |
| `screenshots/` | 測試失敗時的截圖檔案（如有） |

---

#### 📂 bin/ 和 obj/ - 建置輸出
.NET 建置過程產生的二進位檔案和中間檔案。

- `bin/Debug/net8.0-windows/`: 編譯後的執行檔和相依套件
- `obj/`: 編譯過程的中間檔案和快取

**注意**: 這些目錄通常不納入版本控制（.gitignore）

---

### 核心配置檔案

#### App.config
應用程式配置檔，包含測試執行所需的設定。

```xml
<appSettings>
  <add key="AppPath" value="待測應用程式路徑" />
  <add key="WaitTimeout" value="10" />
  <add key="ScreenshotPath" value="./reports/screenshots" />
</appSettings>
```

**重要設定**:
- `AppPath`: WebView2 應用程式的完整路徑
- `WaitTimeout`: 元素等待逾時秒數
- `ScreenshotPath`: 失敗截圖存放位置

---

#### Testcase_shopping_cart_FlaUI_BDD.csproj
.NET 專案檔案，定義專案相依套件和建置設定。

**主要 NuGet 套件**:
```xml
<PackageReference Include="FlaUI.UIA3" Version="4.0.0" />
<PackageReference Include="SpecFlow" Version="3.9.74" />
<PackageReference Include="SpecFlow.NUnit" Version="3.9.74" />
<PackageReference Include="NUnit" Version="4.3.2" />
<PackageReference Include="NUnit3TestAdapter" Version="4.6.0" />
```

**目標框架**: `net8.0-windows`（Windows 專用）

---

### 測試執行流程

```
1. [BeforeScenario] Hook 初始化
   ├── 讀取 App.config 設定
   ├── 啟動待測應用程式（FlaUI Application）
   └── 初始化 Page Objects

2. 執行測試場景 (Feature → Steps)
   ├── Given: 設定前置條件
   ├── When: 執行操作 (透過 Page Objects)
   └── Then: 驗證結果 (Assert)

3. [AfterScenario] Hook 清理
   ├── 如果失敗: 截圖 + 記錄日誌
   ├── 關閉應用程式
   └── 釋放資源

4. 產生測試報告
   ├── junit-results.xml (NUnit 輸出)
   ├── summary_report.md (自訂摘要)
   └── summary_report.html (HTML 檢視)
```

---

### 檔案相依關係

```
ShoppingCart.feature (Gherkin 場景)
       ↓
ShoppingCartSteps.cs (步驟定義)
       ↓
PageObjects/*.cs (頁面操作)
       ↓
FlaUI (UI 自動化引擎)
       ↓
待測應用程式 (WebView2)

支援模組:
- Helpers/ → 提供配置、截圖、日誌
- Hooks/ → 管理測試生命週期
- App.config → 提供執行時設定
```

---

## ✅ 已包含的測試專案

### 1. Testcase_shopping_cart_FlaUI_BDD

**來源**: 從 TestComplete Python 測試案例轉換而來

**包含內容**:
- ✅ 7 個完整的 BDD 測試場景（包含參數化測試）
- ✅ 完整的 Page Object Model 架構
- ✅ SpecFlow 3.9.74 + FlaUI 4.0.0 + NUnit 4.3.2
- ✅ 獨立專案配置（可單獨建置和執行）
- ✅ 完整文件和轉換報告

**測試場景**:
1. TC01 - 加入單一商品
2. TC02 - 加入多種商品  
3. TC03 - 使用加號按鈕增加數量
4. TC04 - 移除商品後購物車回到空狀態
5. TC05 - 加入多項後清空購物車
6. TC06 - 結帳流程驗證
7. 驗證不同商品的加入功能（參數化測試）

---

## ⚠️ 重要提示：需要 Windows 環境

**FlaUI 測試只能在 Windows 上執行**，因為：
- 使用 `net8.0-windows` 目標框架
- 依賴 `Microsoft.WindowsDesktop.App` 運行時
- 需要 Windows UI Automation API

**當前環境**: Linux (Ubuntu) ❌  
**需要環境**: Windows 10/11 ✅

👉 **完整執行指南**: [HOW_TO_RUN_ON_WINDOWS.md](HOW_TO_RUN_ON_WINDOWS.md)

---

## 🌐 測試控制台網頁（三 Tab）

在 Windows 上可啟動網頁控制台，勾選 Feature / Scenario 後執行測試並即時查看進度與結果。

| 頁籤 | 功能 |
|------|------|
| 勾選 Features（藍） | 勾選要執行的 Feature 與 Scenario |
| 執行進度（琥珀） | 查看各 Scenario 狀態與整體執行進度 |
| 測試結果（綠） | 查看 JUnit 結果、通過率，並開啟 HTML 報告 |

```powershell
# 在 Project_FlaUIBDD 目錄
.\start-dashboard.ps1
```

預設網址：`http://localhost:6688/`（**勿用 6666**：Chrome/Edge 會阻擋並顯示 `ERR_UNSAFE_PORT`）

**注意**：執行測試前請確認 Demo Shop 已啟動（`demo/shopping_cart` 的 `python serve.py` 或 WebView 宿主設定正確）。

---

## 🚀 快速開始

### 📋 前置需求（Windows）

1. **.NET 8.0 SDK** - [下載](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Windows 10 或 Windows 11**
3. **Python 3.x** (用於測試網頁伺服器)

### 在 Windows 上執行測試

### 前置需求

- **.NET 8.0+ SDK** 已安裝
- **Windows 作業系統**（FlaUI 需要 Windows UI Automation）
- **Visual Studio** 或 **VS Code**（建議）

### 建置專案

```bash
cd Testcase_shopping_cart_FlaUI_BDD
dotnet restore
dotnet build
```

### 執行測試

```bash
# 執行所有測試（需要 Windows 環境）
dotnet test

# 執行特定場景
dotnet test --filter "TestCategory=ShoppingCart"

# 執行特定場景（加入商品）
dotnet test --filter "TestCategory=AddItem"
```

### 執行測試並產生 Summary Report（推薦）

報告格式依 **[../Project_Testcomplete/report_prompt.md](../Project_Testcomplete/report_prompt.md)**（TestComplete Summary Report 結構）。

```powershell
# 在 Project_FlaUIBDD 目錄（需 Windows）
.\run-tests-and-report.ps1
```

僅從既有 JUnit 重新產生報告：

```powershell
.\generate_report.ps1
```

輸出檔案：

| 檔案 | 說明 |
|------|------|
| `Testcase_shopping_cart_FlaUI_BDD/reports/junit-results.xml` | NUnit JUnit 匯出 |
| `Testcase_shopping_cart_FlaUI_BDD/reports/summary_report.md` | Markdown 摘要 |
| `Testcase_shopping_cart_FlaUI_BDD/reports/summary_report.html` | HTML 摘要 |

---

## 📝 開發指南

### 添加新測試場景

1. 在 `Features/` 目錄編寫 Gherkin 場景
2. 在 `StepDefinitions/` 實作步驟定義
3. 在 `PageObjects/` 添加頁面物件（如需要）
4. 執行 `dotnet build` 生成程式碼
5. 執行 `dotnet test` 驗證測試

### Page Object Model 結構

- **BasePage**: 基礎頁面類別，提供通用方法
- **ProductListPage**: 產品列表頁面操作
- **ShoppingCartPage**: 購物車頁面操作和驗證

---

## 🔧 技術棧

| 技術 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0+ | 開發平台 |
| SpecFlow | 3.9.74 | BDD 測試框架 |
| FlaUI | 4.0.0 | Windows UI 自動化 |
| NUnit | 4.3.2 | 測試執行框架 |
| C# | 11+ | 程式語言 |

---

## 📚 相關文件

- [Testcase_shopping_cart_FlaUI_BDD/README.md](Testcase_shopping_cart_FlaUI_BDD/README.md) - 購物車測試專案詳細說明
- [HOW_TO_RUN_ON_WINDOWS.md](HOW_TO_RUN_ON_WINDOWS.md) - Windows 執行指南
- [Testcase_shopping_cart_FlaUI_BDD/CONVERSION_REPORT.md](Testcase_shopping_cart_FlaUI_BDD/CONVERSION_REPORT.md) - TestComplete 轉換報告
- [../Project_Testcomplete/report_prompt.md](../Project_Testcomplete/report_prompt.md) - Summary Report 格式規格
- [../Project_Testcomplete/README.md](../Project_Testcomplete/README.md) - TestComplete 專案與報告流程

---

## ⚠️ 注意事項

1. **Windows 環境限制**: FlaUI 測試必須在 Windows 環境執行
2. **當前環境**: Linux dev container 可以編輯和建置，但無法執行測試
3. **AutomationId**: 確保目標應用程式元素有設定 AutomationId 屬性
4. **應用程式路徑**: 執行測試前需在 `App.config` 配置應用程式路徑

---

## 🎯 專案狀態

✅ **建置成功** - 0 錯誤  
✅ **專案結構完整** - 所有必要檔案已就緒  
✅ **文件齊全** - README + 轉換報告  
✅ **準備就緒** - 可在 Windows 環境執行測試  

---

**最後更新**: 2026-05-27  
**專案位置**: `/workspaces/SQA_AI_Automation/Project_FlaUIBDD/`
