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
│   ├── App.config                        # 應用程式配置
│   ├── *.csproj                          # 專案檔案
│   └── README.md                         # 專案說明
├── INSTALLATION_REPORT.md               # 安裝報告
└── README.md                            # 本文件
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
- [Testcase_shopping_cart_FlaUI_BDD/CONVERSION_REPORT.md](Testcase_shopping_cart_FlaUI_BDD/CONVERSION_REPORT.md) - TestComplete 轉換報告
- [INSTALLATION_REPORT.md](INSTALLATION_REPORT.md) - 套件安裝報告

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
