# 如何在 Windows 上執行 FlaUI BDD 測試

## 📋 前置需求

### 1. 安裝 .NET 8.0 SDK (Windows)
- 下載：https://dotnet.microsoft.com/download/dotnet/8.0
- 選擇：**.NET 8.0 SDK** (Windows x64)
- 安裝後驗證：
  ```cmd
  dotnet --version
  ```
  應顯示 8.0.x

### 2. 安裝 Git (Windows)
- 下載：https://git-scm.com/download/win
- 選擇預設選項安裝

---

## 🚀 執行步驟

### 步驟 1：複製專案到 Windows 機器

#### 方式 A：使用 Git Clone
```cmd
git clone https://github.com/PorscheWen/SQA_AI_Automation.git
cd SQA_AI_Automation\Project_FlaUIBDD\Testcase_shopping_cart_FlaUI_BDD
```

#### 方式 B：下載 ZIP
1. 前往：https://github.com/PorscheWen/SQA_AI_Automation
2. 點擊 **Code** → **Download ZIP**
3. 解壓縮後進入目錄：
   ```cmd
   cd SQA_AI_Automation\Project_FlaUIBDD\Testcase_shopping_cart_FlaUI_BDD
   ```

---

### 步驟 2：啟動測試網頁伺服器

開啟 **命令提示字元 (CMD)** 或 **PowerShell**：

```cmd
cd SQA_AI_Automation\demo\shopping_cart
python -m http.server 8888
```

或使用 Python 內建伺服器：
```cmd
python serve.py
```

**保持此視窗開啟**，伺服器將運行在 `http://localhost:8888/`

---

### 步驟 3：還原 NuGet 套件

開啟**新的**命令提示字元視窗：

```cmd
cd SQA_AI_Automation\Project_FlaUIBDD\Testcase_shopping_cart_FlaUI_BDD
dotnet restore
```

預期輸出：
```
Restored Testcase_shopping_cart_FlaUI_BDD.csproj (in 2 sec).
```

---

### 步驟 4：建置專案

```cmd
dotnet build --configuration Release
```

預期輸出：
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

### 步驟 5：執行 BDD 測試

```cmd
dotnet test --verbosity normal
```

---

## 📊 預期測試結果

### 成功執行範例

```
Test run for Testcase_shopping_cart_FlaUI_BDD.dll (.NET 8.0)
Starting test discovery...
Test discovery finished.

Running tests...
  ✅ TC01_加入單一商品到購物車 - Passed (2.3 sec)
  ✅ TC02_加入多種商品到購物車 - Passed (3.1 sec)
  ✅ TC03_增加購物車商品數量 - Passed (2.8 sec)
  ✅ TC04_從購物車移除商品 - Passed (2.5 sec)
  ✅ TC05_清空購物車 - Passed (2.2 sec)
  ✅ TC06_測試結帳流程 - Passed (2.9 sec)
  ✅ TC07_參數化測試_加入不同商品 (蘋果) - Passed (2.4 sec)
  ✅ TC07_參數化測試_加入不同商品 (香蕉) - Passed (2.3 sec)
  ✅ TC07_參數化測試_加入不同商品 (牛奶) - Passed (2.4 sec)

Total tests: 9
     Passed: 9
     Failed: 0
    Skipped: 0
 Total time: 23.1 seconds
```

---

## 🐛 疑難排解

### 問題 1：找不到 .NET 8.0 運行時
**錯誤訊息：**
```
Framework 'Microsoft.NETCore.App', version '8.0.0' not found
```

**解決方法：**
安裝 .NET 8.0 SDK（包含運行時）：
https://dotnet.microsoft.com/download/dotnet/8.0

### 問題 2：找不到 Windows Desktop 運行時
**錯誤訊息：**
```
Framework 'Microsoft.WindowsDesktop.App', version '8.0.0' not found
```

**解決方法：**
安裝 .NET 8.0 Desktop Runtime：
https://dotnet.microsoft.com/download/dotnet/8.0/runtime

選擇：**Desktop Runtime 8.0.x (Windows x64)**

### 問題 3：無法連接到網頁
**錯誤訊息：**
```
Unable to find element or window not responding
```

**解決方法：**
1. 確認網頁伺服器正在運行：
   ```cmd
   netstat -ano | findstr :8888
   ```
2. 手動開啟瀏覽器測試：http://localhost:8888/
3. 檢查防火牆設定是否封鎖 Python

### 問題 4：元素定位失敗
**可能原因：**
- 網頁載入時間過長
- 瀏覽器版本不相容

**解決方法：**
1. 修改 `App.config` 增加等待時間：
   ```xml
   <add key="DefaultTimeout" value="30000" />
   ```
2. 使用最新版 Chrome 或 Edge 瀏覽器

---

## 📸 截圖位置

測試失敗時會自動截圖，位置：
```
Testcase_shopping_cart_FlaUI_BDD\bin\Debug\net8.0-windows\Screenshots\
```

檔名格式：
```
TC01_加入單一商品到購物車_20260527_143022.png
```

---

## 🔧 進階設定

### 只執行特定測試

```cmd
dotnet test --filter "Name~TC01"
```

### 產生測試報告

```cmd
dotnet test --logger "trx;LogFileName=test_results.trx"
```

報告位置：`TestResults\test_results.trx`

### 調整詳細程度

```cmd
dotnet test --verbosity detailed
```

---

## 📝 測試場景說明

| 測試編號 | 場景描述 | 驗證項目 |
|---------|---------|---------|
| TC01 | 加入單一商品（蘋果）| 件數=1, 總計=NT$ 30 |
| TC02 | 加入多種商品 | 件數=3, 總計=NT$ 105 |
| TC03 | 增加商品數量 | 點擊 + 按鈕，數量增加 |
| TC04 | 移除商品 | 購物車清空 |
| TC05 | 清空購物車 | 總計=NT$ 0 |
| TC06 | 結帳流程 | 顯示結帳訊息 |
| TC07 | 參數化測試 | 蘋果/香蕉/牛奶各一次 |

---

## 🌐 相關資源

- **專案 README**：`Project_FlaUIBDD/README.md`
- **Feature 檔案**：`Features/ShoppingCart.feature`
- **Page Objects**：`PageObjects/` 目錄
- **FlaUI 官方文件**：https://github.com/FlaUI/FlaUI
- **SpecFlow 文件**：https://docs.specflow.org/

---

## ❓ 常見問題

### Q: 可以在 Linux 上執行嗎？
**A:** 不行。FlaUI 依賴 Windows UI Automation API，只能在 Windows 上運行。

### Q: 可以使用 macOS 嗎？
**A:** 不行。同樣需要 Windows 環境。

### Q: 有替代方案嗎？
**A:** 是的！可以使用 Selenium 創建跨平台測試。請參考專案中的 `demo/shopping_website/` 目錄。

### Q: 測試需要多久？
**A:** 全部 9 個測試約需 20-25 秒完成。

### Q: 可以整合到 CI/CD 嗎？
**A:** 可以！使用 GitHub Actions 的 Windows runner：
```yaml
runs-on: windows-latest
```

---

## 📞 技術支援

如遇到問題，請檢查：
1. .NET 版本是否正確（`dotnet --info`）
2. 網頁伺服器是否運行（`netstat -ano | findstr :8888`）
3. 防毒軟體是否封鎖測試
4. Windows 版本（建議 Windows 10/11）

---

**最後更新**：2026-05-27
**專案版本**：1.0
**測試框架**：SpecFlow 3.9 + FlaUI 4.0 + NUnit 4.3
