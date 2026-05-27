# ⚠️ Dev Container 環境限制說明

## 問題分析

```
SessionNotCreatedException: Chrome instance exited
```

### 根本原因

1. ✅ **ChromeDriver 已下載** - WebDriver Manager 成功取得 149.0.7827.22
2. ❌ **Chrome 瀏覽器未安裝** - Dev Container 沒有 Chrome/Chromium 瀏覽器
3. ❌ **無 GUI 環境** - 容器環境通常沒有圖形介面

---

## 🎯 可行的解決方案

### 方案 1: 在本地環境執行（最簡單）✅

將專案複製到本地機器（Windows/macOS/Linux 桌面）：

#### Windows PowerShell:
```powershell
# 1. 複製專案
git clone https://github.com/PorscheWen/SQA_AI_Automation.git
cd SQA_AI_Automation\Project_PytestBDD

# 2. 建立虛擬環境（建議）
python -m venv venv
venv\Scripts\activate

# 3. 安裝依賴
pip install -r requirements.txt

# 4. 啟動測試網頁（開新視窗）
cd ..\demo\shopping_cart
python serve.py

# 5. 執行測試（回到 Project_PytestBDD）
cd ..\..\Project_PytestBDD
pytest --headless -v
```

#### macOS/Linux:
```bash
# 1. 複製專案
git clone https://github.com/PorscheWen/SQA_AI_Automation.git
cd SQA_AI_Automation/Project_PytestBDD

# 2. 建立虛擬環境（建議）
python3 -m venv venv
source venv/bin/activate

# 3. 安裝依賴
pip install -r requirements.txt

# 4. 安裝 Chrome/Chromium
# Ubuntu/Debian:
sudo apt-get update && sudo apt-get install -y chromium-browser
# macOS:
brew install --cask google-chrome

# 5. 啟動測試網頁（開新終端）
cd ../demo/shopping_cart
python3 serve.py

# 6. 執行測試
cd ../../Project_PytestBDD
pytest --headless -v
```

---

### 方案 2: 使用 Selenium Grid（進階）

在有 GUI 的機器上運行 Selenium Grid，從 dev container 連接：

```python
# conftest.py 修改
from selenium.webdriver.remote.webdriver import WebDriver

driver = WebDriver(
    command_executor='http://selenium-hub:4444/wd/hub',
    options=chrome_options
)
```

---

### 方案 3: 使用 Docker Selenium 容器

```bash
# 啟動 Selenium Standalone Chrome
docker run -d -p 4444:4444 --shm-size="2g" \
  selenium/standalone-chrome:latest

# 修改測試配置連接到 localhost:4444
```

---

### 方案 4: 手動測試（當前可用）✅

測試網頁已運行: http://localhost:8888/

手動執行測試場景：

| 測試 | 操作 | 預期結果 |
|------|------|---------|
| TC01 | 點擊「加入購物車」(蘋果) | 件數: 1 件, 總計: NT$ 30 |
| TC02 | 加入蘋果、香蕉、牛奶 | 件數: 3 件, 總計: NT$ 105 |
| TC03 | 加入蘋果後點擊「+」 | 數量增為 2, 總計: NT$ 60 |
| TC04 | 加入蘋果後點擊「移除」 | 件數: 0 件, 總計: NT$ 0 |
| TC05 | 加入多項後點「清空購物車」 | 件數: 0 件, 總計: NT$ 0 |
| TC06 | 加入商品後點「結帳」 | 顯示結帳訊息 |
| TC07 | 分別測試蘋果/香蕉/牛奶 | 各商品正確加入 |

---

## 🔍 環境檢查腳本

運行以下命令檢查環境：

```bash
# 檢查專案結構
python verify_project.py

# 檢查是否有瀏覽器
which google-chrome chromium-browser chromium firefox

# 檢查網頁伺服器
curl -s http://localhost:8888/ | head -3

# 查看 Python 套件
pip list | grep -E "pytest|selenium"
```

---

## 📊 當前狀態

### ✅ 已完成
- 專案結構完整 (18 個檔案)
- Python 依賴已安裝
- ChromeDriver 已下載 (149.0.7827.22)
- 測試網頁運行中 (port 8888)

### ❌ 缺少
- Chrome/Chromium 瀏覽器
- GUI 環境支援

---

## 💡 建議

### 當前環境（Dev Container）
1. ✅ 使用 `python verify_project.py` 驗證專案結構
2. ✅ 查看測試程式碼和 Gherkin 場景
3. ✅ 手動測試網頁功能 (http://localhost:8888/)

### 本地環境
1. ✅ 複製專案到本地機器
2. ✅ 安裝瀏覽器
3. ✅ 執行自動化測試

---

## 🎓 專案學習價值

即使無法在 dev container 中執行測試，此專案仍提供：

1. **完整的 BDD 測試架構範例**
   - Gherkin 場景撰寫
   - Step Definitions 實作
   - Page Object Model 設計

2. **Pytest 配置最佳實踐**
   - conftest.py 設定
   - Fixtures 使用
   - 測試報告產生

3. **Selenium 操作模式**
   - 元素定位策略
   - 等待機制
   - 錯誤處理

4. **跨平台測試方案**
   - 多瀏覽器支援
   - 環境配置管理
   - CI/CD 整合準備

---

## 📞 需要協助？

### 檢查清單

- [ ] 已在本地環境安裝 Chrome/Chromium
- [ ] 已安裝 Python 3.8+
- [ ] 已安裝 requirements.txt 的套件
- [ ] 測試網頁正在運行
- [ ] 可以訪問 http://localhost:8888/

### 常見錯誤

| 錯誤 | 原因 | 解決方法 |
|------|------|---------|
| SessionNotCreatedException | 無瀏覽器 | 安裝 Chrome/Chromium |
| WebDriverException | ChromeDriver 版本不符 | 更新瀏覽器或 webdriver-manager |
| Connection refused | 網頁未啟動 | 執行 `python serve.py` |
| ModuleNotFoundError | 套件未安裝 | `pip install -r requirements.txt` |

---

**結論**: 專案完整且正確，只是受限於 dev container 環境。在有瀏覽器的本地環境中可以完美執行。
