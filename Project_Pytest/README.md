# Project Pytest — 購物車自動化測試

本專案將 TestComplete Python 測試腳本直接轉換為標準 **pytest** 測試，  
不依賴 BDD / Gherkin，測試結構與 TestComplete 函數命名保持一致。

---

## 對應關係

| TestComplete | pytest |
|---|---|
| `Project_Testcomplete/Testcomplete_testcase/Testcase_shopping_cart` | `tests/test_shopping_cart.py`（TC01–TC06）|
| `demo/shopping_website/web_shopping_cart_test_suite.py` | `tests/test_full_suite.py`（FT01–FT06、NT01–NT03、CT01–CT02）|

### 核心 API 對映

| TestComplete | ShoppingCartPage | 說明 |
|---|---|---|
| `control_click_testid(id)` | `page._js_click(id)` | JS 點擊（對隱藏元素有效）|
| `control_get_text_by_testid(id)` | `page._js_text(id)` | JS textContent |
| `control_reset_cart()` | `page.reset_cart()` | 若非空則清空 |
| `control_verify_text(...)` | `assert page.get_*() == expected` | pytest assert |
| `Log.Message / Log.Error` | `print` / pytest assert | 測試輸出 |

---

## 專案結構

```
Project_Pytest/
├── conftest.py                # 全域 fixtures（browser、base_url）
├── pytest.ini                 # pytest 設定
├── requirements.txt           # Python 依賴
├── page_objects/
│   ├── base_page.py           # 通用 Selenium 封裝
│   └── shopping_cart_page.py  # 購物車頁面物件（data-testid 選擇器）
├── tests/
│   ├── test_shopping_cart.py  # TC01–TC06（核心功能）
│   └── test_full_suite.py     # FT01–FT06、NT01–NT03、CT01–CT02
└── reports/                   # 測試報告輸出目錄（git ignore）
```

---

## 快速開始

### 1. 啟動 Demo 應用

```bash
cd demo/shopping_cart
python serve.py
# 確認 http://localhost:8888 可正常開啟
```

### 2. 安裝依賴

```bash
cd Project_Pytest
pip install -r requirements.txt
```

### 3. 執行測試

```bash
# 所有測試（無頭 Chrome，自動偵測 ChromeDriver）
pytest --headless

# 只執行 TC01–TC06
pytest tests/test_shopping_cart.py --headless

# 只執行完整套件
pytest tests/test_full_suite.py --headless

# 以標記篩選
pytest -m functional --headless
pytest -m negative --headless
pytest -m compat --headless

# 產生 HTML 報告
pytest --headless --html=reports/report.html --self-contained-html

# 指定瀏覽器 / URL
pytest --browser=firefox --base-url=http://localhost:8888
```

---

## 測試案例對照表

### tests/test_shopping_cart.py（TC01–TC06）

| ID | 函數 | 說明 | TestComplete 對應 |
|----|------|------|------------------|
| TC01 | `test_tc01_add_single_item` | 加入單一商品 | `testcase_add_single_item` |
| TC02 | `test_tc02_add_multiple_items` | 加入多種商品 | `testcase_add_multiple_items` |
| TC03 | `test_tc03_increase_quantity` | 增加數量 | `testcase_increase_quantity` |
| TC04 | `test_tc04_remove_item` | 移除商品 | `testcase_remove_item` |
| TC05 | `test_tc05_clear_cart` | 清空購物車 | `testcase_clear_cart` |
| TC06 | `test_tc06_checkout` | 結帳流程 | `testcase_checkout` |

### tests/test_full_suite.py（FT01–FT06、NT01–NT03、CT01–CT02）

| ID | 類別 | 說明 | TestComplete 對應 |
|----|------|------|------------------|
| FT01 | functional | 加入單一商品 | `testcase_ft01_add_single_item` |
| FT02 | functional | 加入多種商品 | `testcase_ft02_add_multiple_items` |
| FT03 | functional | 增加數量 | `testcase_ft03_increase_quantity` |
| FT04 | functional | 移除商品 | `testcase_ft04_remove_item` |
| FT05 | functional | 清空購物車 | `testcase_ft05_clear_cart` |
| FT06 | functional | 結帳成功 + Modal 關閉 | `testcase_ft06_checkout_success` |
| NT01 | negative | 空購物車按結帳 | `testcase_nt01_checkout_empty_cart` |
| NT02 | negative | 數量減至零自動移除 | `testcase_nt02_decrease_to_zero` |
| NT03 | negative | 空購物車重複清空 | `testcase_nt03_clear_empty_cart` |
| CT01 | compat | Chrome 瀏覽器相容 | `testcase_ct01_compat_chrome` |
| CT02 | compat | Edge 瀏覽器相容 | `testcase_ct02_compat_edge` |

> **CT02** 在 Edge 未安裝的環境會自動略過（`pytest.mark.skipif`）。

---

## 設計說明

### data-testid 選擇器

頁面物件全部使用 `data-testid` 屬性定位元素，  
與 TestComplete 腳本的 `FindChildByXPath("//*[@data-testid='...']")` 保持完全一致。

### JavaScript 操作

`ShoppingCartPage` 透過 JS 執行點擊（`el.click()`）與讀取（`el.textContent`），  
這樣即使按鈕因購物車空而被父容器 `hidden` 屬性隱藏，操作仍然有效，  
與 TestComplete 的行為一致（NT01、NT03 的空購物車操作場景）。
