# ⚡ 快速啟動指南

## ⚠️ Dev Container 環境限制

**當前環境**: Dev Container（無 GUI/瀏覽器）  
**完整測試**: 需要在本地機器執行

### 🧪 在當前環境可以做的：

```bash
# 1. 驗證專案結構
python verify_project.py

# 2. 執行 E2E 測試（需要瀏覽器）
python test_with_browser.py

# 3. 查看測試場景
cat features/shopping_cart.feature

# 4. 手動測試網頁
# 在瀏覽器開啟: http://localhost:8888/
```

---

## 🎯 完整執行指南（本地環境）

### 方式 1: 在本地機器執行（推薦）✅

將專案複製到本地環境：

```bash
# 複製專案
git clone https://github.com/PorscheWen/SQA_AI_Automation.git
cd SQA_AI_Automation/Project_PytestBDD

# 安裝依賴
pip install -r requirements.txt

# 啟動測試網頁（另開終端）
cd ../demo/shopping_cart
python serve.py

# 執行測試
cd ../../Project_PytestBDD
pytest --headless
```

---

## 🔧 常用命令

```bash
# Chrome headless 模式（推薦）
pytest --headless

# Chrome 視窗模式
pytest

# Firefox headless 模式
pytest --browser=firefox --headless

# 詳細輸出
pytest -v --headless

# 執行特定測試
pytest -k "TC01" --headless

# 產生 HTML 報告
pytest --headless --html=reports/report.html
```

---

## 📊 預期結果

```
======================== test session starts ========================
collected 9 items

step_definitions/test_shopping_cart_steps.py::test_tc01_加入單一商品到購物車 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc02_加入多種商品到購物車 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc03_增加購物車商品數量 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc04_從購物車移除商品 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc05_清空購物車 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc06_測試結帳流程 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc07_蘋果 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc07_香蕉 PASSED
step_definitions/test_shopping_cart_steps.py::test_tc07_牛奶 PASSED

====================== 9 passed in 15.23s =======================
```

---

## 🐛 疑難排解

### ❌ ModuleNotFoundError: No module named 'pytest_bdd'

```bash
pip install -r requirements.txt
```

### ❌ WebDriverException: Message: 'chromedriver' executable needs to be in PATH

```bash
pip install webdriver-manager
```

### ❌ Connection refused [::1]:8888

```bash
# 確認網頁伺服器正在運行
cd ../demo/shopping_cart
python serve.py
```

---

## 📸 查看報告

```bash
# HTML 報告
open reports/report.html  # macOS
xdg-open reports/report.html  # Linux
start reports/report.html  # Windows

# 失敗截圖
ls -la reports/screenshots/
```

---

## ✅ 環境檢查

```bash
# 檢查 Python 版本
python --version  # 應該 >= 3.8

# 檢查 pip
pip --version

# 檢查已安裝套件
pip list | grep -E "pytest|selenium"

# 檢查網頁是否可訪問
curl http://localhost:8888/
```

---

**提示**: 使用 `--headless` 模式可加快測試執行速度！
