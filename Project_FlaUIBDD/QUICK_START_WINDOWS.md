# ⚡ FlaUI BDD 測試 - Windows 快速啟動

## 🎯 一鍵執行（Windows PowerShell）

```powershell
# 1️⃣ 進入專案目錄
cd Project_FlaUIBDD\Testcase_shopping_cart_FlaUI_BDD

# 2️⃣ 啟動測試網頁（另開視窗）
cd ..\..\demo\shopping_cart
python serve.py

# 3️⃣ 執行測試
cd ..\..\Project_FlaUIBDD\Testcase_shopping_cart_FlaUI_BDD
dotnet test
```

---

## 📊 預期結果

```
✅ 通過 9 個測試
⏱️  執行時間: ~25 秒
📸 失敗截圖: bin\Debug\net8.0-windows\Screenshots\
```

---

## 🔧 常用命令

| 功能 | 命令 |
|------|------|
| **建置專案** | `dotnet build` |
| **執行全部測試** | `dotnet test` |
| **執行特定測試** | `dotnet test --filter "Name~TC01"` |
| **詳細輸出** | `dotnet test --verbosity detailed` |
| **產生報告** | `dotnet test --logger trx` |
| **還原套件** | `dotnet restore` |

---

## 📦 環境需求

- ✅ Windows 10/11
- ✅ .NET 8.0 SDK
- ✅ Python 3.x

---

## ❌ Linux/macOS 無法執行

FlaUI 使用 Windows UI Automation，必須在 Windows 環境執行。

**替代方案**: 使用 Selenium（跨平台）

---

## 🆘 快速疑難排解

### 缺少 .NET 8.0
```bash
dotnet --version  # 檢查版本
```
👉 安裝：https://dotnet.microsoft.com/download/dotnet/8.0

### 缺少 Desktop Runtime
```
Framework 'Microsoft.WindowsDesktop.App' not found
```
👉 安裝：.NET Desktop Runtime 8.0

### 網頁無法開啟
```bash
netstat -ano | findstr :8888  # 檢查埠號
```
👉 確認 Python 伺服器正在運行

---

## 📖 詳細文件

- [完整執行指南](HOW_TO_RUN_ON_WINDOWS.md)
- [專案 README](README.md)
- [測試場景](Testcase_shopping_cart_FlaUI_BDD/Features/ShoppingCart.feature)
