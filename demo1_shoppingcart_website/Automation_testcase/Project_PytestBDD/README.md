# 🐍 Pytest BDD - 購物車自動化測試

使用 **Pytest BDD + Selenium** 實現的購物車功能測試專案，支援跨平台執行（Linux/Windows/macOS）。

---

## 📋 專案特色

✅ **BDD 測試框架** - 使用 Gherkin 語法撰寫測試場景  
✅ **跨平台支援** - Linux、Windows、macOS 皆可執行  
✅ **Page Object Model** - 清晰的頁面物件架構  
✅ **中文測試場景** - 使用中文撰寫的可讀性高的測試描述  
✅ **自動截圖** - 測試失敗時自動擷取截圖  
✅ **多瀏覽器支援** - Chrome、Firefox  
✅ **HTML 報告** - 自動產生美觀的測試報告  

---

## 📁 專案結構

```
Project_PytestBDD/
├── features/                       # Gherkin 測試場景
│   └── shopping_cart.feature      # 購物車測試場景
├── step_definitions/              # 步驟定義
│   └── test_shopping_cart_steps.py
├── page_objects/                  # Page Object Model
│   ├── base_page.py              # 基礎頁面物件
│   ├── product_list_page.py      # 商品列表頁面
│   └── shopping_cart_page.py     # 購物車頁面
├── helpers/                       # 輔助工具
├── reports/                       # 測試報告
│   └── screenshots/              # 失敗截圖
├── conftest.py                   # Pytest 配置和 Fixtures
├── pytest.ini                    # Pytest 設定檔
├── requirements.txt              # Python 依賴套件
└── README.md                     # 本文件
```

---

## 🚀 快速開始

### 1️⃣ 安裝依賴

```bash
# 進入專案目錄
cd Project_PytestBDD

# 安裝 Python 套件
pip install -r requirements.txt
```

### 2️⃣ 啟動測試網頁

開啟新的終端視窗：

```bash
# 啟動購物車網頁伺服器
cd ../demo/shopping_cart
python serve.py
```

網頁將運行在：http://localhost:8888/

### 3️⃣ 執行測試

```bash
# 回到 Project_PytestBDD 目錄
cd ../../Project_PytestBDD

# 執行所有測試（Chrome headless 模式）
pytest --headless

# 執行所有測試（Chrome 視窗模式）
pytest

# 執行所有測試（Firefox headless 模式）
pytest --browser=firefox --headless
```

---

## 📊 測試場景

| 編號 | 場景名稱 | 測試步驟 | 驗證項目 |
|------|---------|---------|---------|
| TC01 | 加入單一商品到購物車 | 加入蘋果 | 件數=1, 總計=NT$ 30 |
| TC02 | 加入多種商品到購物車 | 加入蘋果、香蕉、牛奶 | 件數=3, 總計=NT$ 105 |
| TC03 | 增加購物車商品數量 | 加入蘋果後點擊 + | 數量=2, 總計=NT$ 60 |
| TC04 | 從購物車移除商品 | 加入蘋果後移除 | 件數=0, 總計=NT$ 0 |
| TC05 | 清空購物車 | 加入多項後清空 | 件數=0, 總計=NT$ 0 |
| TC06 | 測試結帳流程 | 加入商品後結帳 | 顯示結帳訊息 |
| TC07 | 驗證不同商品的加入功能 | 分別測試蘋果/香蕉/牛奶 | 各商品正確加入 |

**總計**: 9 個測試（包含參數化測試的 3 個變化）

---

## 🔧 進階用法

### 執行特定測試

```bash
# 執行特定場景（使用關鍵字）
pytest -k "TC01"
pytest -k "加入單一商品"

# 執行特定功能檔案
pytest --feature features/shopping_cart.feature
```

### 自訂設定

```bash
# 指定瀏覽器
pytest --browser=chrome      # Chrome（預設）
pytest --browser=firefox     # Firefox

# 無頭模式
pytest --headless

# 自訂網址
pytest --base-url=http://localhost:9000

# 詳細輸出
pytest -v

# 產生 HTML 報告
pytest --html=reports/report.html
```

### 平行執行

```bash
# 使用 4 個 worker 平行執行
pytest -n 4
```

---

## 📸 測試報告

### HTML 報告

執行測試後會自動產生 HTML 報告：

```
reports/report.html
```

用瀏覽器開啟查看詳細測試結果。

### 失敗截圖

測試失敗時會自動截圖，儲存位置：

```
reports/screenshots/FAILED_<測試名稱>_<時間戳>.png
```

---

## 🛠️ 開發指南

### 新增測試場景

1. **編輯 Feature 檔案**
   ```gherkin
   # features/shopping_cart.feature
   場景: 新的測試場景
     當 執行某個動作
     那麼 驗證某個結果
   ```

2. **實作步驟定義**
   ```python
   # step_definitions/test_shopping_cart_steps.py
   @when('執行某個動作')
   def execute_action(pages):
       # 實作邏輯
       pass
   ```

### 新增頁面物件

```python
# page_objects/new_page.py
from page_objects.base_page import BasePage
from selenium.webdriver.common.by import By

class NewPage(BasePage):
    ELEMENT = (By.ID, "element-id")
    
    def __init__(self, driver):
        super().__init__(driver)
    
    def click_element(self):
        self.click_element(self.ELEMENT)
```

---

## 🐛 疑難排解

### 問題 1: 找不到 ChromeDriver

**解決方法**:
```bash
# 安裝 webdriver-manager（已包含在 requirements.txt）
pip install webdriver-manager
```

### 問題 2: 網頁無法連線

**檢查**:
- 確認測試網頁伺服器正在運行（http://localhost:8888/）
- 檢查防火牆設定

**解決方法**:
```bash
# 重新啟動網頁伺服器
cd ../demo/shopping_cart
python serve.py
```

### 問題 3: 測試執行緩慢

**解決方法**:
```bash
# 使用 headless 模式
pytest --headless

# 使用平行執行
pytest -n auto
```

### 問題 4: 元素定位失敗

**檢查**:
- 元素 ID 是否正確
- 頁面是否完全載入

**除錯**:
```bash
# 不使用 headless 模式，觀察瀏覽器行為
pytest -v
```

---

## 📦 系統需求

| 項目 | 版本 | 說明 |
|------|------|------|
| Python | 3.8+ | 程式語言 |
| Pytest | 7.4+ | 測試框架 |
| Pytest-BDD | 6.1+ | BDD 擴充 |
| Selenium | 4.15+ | WebDriver |
| Chrome/Firefox | 最新版 | 瀏覽器 |

---

## 🔄 與 FlaUI BDD 專案的比較

| 特性 | Pytest BDD (本專案) | FlaUI BDD |
|------|-------------------|-----------|
| 平台 | ✅ Linux/Windows/macOS | ❌ 僅 Windows |
| 語言 | Python | C# |
| 框架 | Pytest + Selenium | SpecFlow + FlaUI |
| 測試對象 | Web 應用程式 | Windows 應用程式 |
| 執行速度 | 快 | 中等 |
| 學習曲線 | 低 | 中等 |

---

## 📚 相關文件

- [Pytest 官方文件](https://docs.pytest.org/)
- [Pytest-BDD 文件](https://pytest-bdd.readthedocs.io/)
- [Selenium 文件](https://www.selenium.dev/documentation/)
- [Gherkin 語法](https://cucumber.io/docs/gherkin/)

---

## 🎯 測試策略

### 測試層級

1. **煙霧測試** - 快速驗證核心功能
2. **回歸測試** - 完整功能驗證
3. **UI 測試** - 使用者介面驗證

### CI/CD 整合

可以整合到 GitHub Actions：

```yaml
name: Pytest BDD Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-python@v4
        with:
          python-version: '3.11'
      - name: Install dependencies
        run: |
          pip install -r requirements.txt
      - name: Start web server
        run: |
          cd demo/shopping_cart
          python serve.py &
          sleep 5
      - name: Run tests
        run: |
          cd Project_PytestBDD
          pytest --headless --html=reports/report.html
      - name: Upload reports
        uses: actions/upload-artifact@v3
        with:
          name: test-reports
          path: Project_PytestBDD/reports/
```

---

## 💡 最佳實踐

1. ✅ 使用 Page Object Model 分離測試邏輯和頁面操作
2. ✅ 每個測試保持獨立，不依賴執行順序
3. ✅ 使用有意義的變數和函數命名
4. ✅ 測試失敗時提供清楚的錯誤訊息
5. ✅ 定期更新依賴套件
6. ✅ 使用 headless 模式加速 CI/CD 執行

---

## 📞 支援

遇到問題？查看：
- 專案 Issue 追蹤器
- Pytest BDD 官方文件
- Selenium 社群論壇

---

**專案版本**: 1.0  
**最後更新**: 2026-05-27  
**Python 版本**: 3.8+  
**測試框架**: Pytest BDD + Selenium
