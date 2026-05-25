# Demo Shop 購物車 — 自動化測試計劃

| 項目 | 內容 |
|------|------|
| 被測系統 | Demo Shop 購物車（`http://localhost:8888/`） |
| 測試工具 | TestComplete（Python Script） |
| 測試腳本 | `web_shopping_cart_test_suite.py` |
| 測試案例數 | **11**（黑箱功能 6 + 負面 3 + 相容 2） |
| 前置條件 | 執行 `demo/shopping_cart/serve.py` 啟動本地伺服器 |

※ CT01 為跨瀏覽器相容測試，內含 **Chrome** 與 **Edge** 兩種瀏覽器驗證。

---

## 測試範圍

| 功能模組 | 說明 |
|----------|------|
| 商品列表 | 蘋果 NT$30、香蕉 NT$20、鮮奶 NT$55 |
| 加入購物車 | 單品 / 多品加入 |
| 數量控制 | +/- 調整、減至 0 移除 |
| 購物車管理 | 移除、清空 |
| 結帳 | 成功 Modal、關閉後清空 |
| 空狀態 | 空購物車提示訊息 |

---

## 測試案例一覽

### 黑箱功能測試（6）

| ID | 案例名稱 | 測試步驟 | 預期結果 | 對應函式 |
|----|----------|----------|----------|----------|
| FT01 | 加入單一商品 | 清空購物車 → 點「加入購物車」（蘋果） | 件數 1 件、總計 NT$ 30、蘋果數量 1 | `test_ft01_add_single_item` |
| FT02 | 加入多種商品 | 清空 → 依序加入蘋果、香蕉、鮮奶 | 件數 3 件、總計 NT$ 105 | `test_ft02_add_multiple_items` |
| FT03 | 增加商品數量 | 清空 → 加入香蕉 → 按 + | 香蕉數量 2、件數 2 件、總計 NT$ 40 | `test_ft03_increase_quantity` |
| FT04 | 移除商品 | 清空 → 加入鮮奶 → 按「移除」 | 件數 0 件、顯示空購物車訊息 | `test_ft04_remove_item` |
| FT05 | 清空購物車 | 清空 → 加入蘋果、鮮奶 → 按「清空購物車」 | 件數 0 件、總計 NT$ 0 | `test_ft05_clear_cart` |
| FT06 | 結帳成功流程 | 清空 → 加入蘋果、香蕉 → 結帳 → 關閉 Modal | 顯示成功訊息、總金額 NT$ 50；關閉後購物車清空 | `test_ft06_checkout_success` |

### 負面測試（3）

| ID | 案例名稱 | 測試步驟 | 預期結果 | 對應函式 |
|----|----------|----------|----------|----------|
| NT01 | 空購物車結帳 | 確保購物車為空 → 按「結帳」 | 結帳 Modal **不**出現，仍為空購物車 | `test_nt01_checkout_empty_cart` |
| NT02 | 數量減至零 | 加入蘋果 → 按「-」 | 商品被移除，件數 0 件、顯示空購物車訊息 | `test_nt02_decrease_to_zero` |
| NT03 | 空購物車重複清空 | 確保購物車為空 → 連續按「清空購物車」2 次 | 維持 0 件，頁面無異常 | `test_nt03_clear_empty_cart` |

### 相容測試（2）

| ID | 案例名稱 | 測試步驟 | 預期結果 | 對應函式 |
|----|----------|----------|----------|----------|
| CT01 | Chrome 相容 | Chrome 1280×800 開啟首頁 → 驗證標題 → 加入香蕉 | 標題正確、件數 1 件、總計 NT$ 20 | `test_ct01_compat_chrome` |
| CT02 | Edge 相容 | Edge 1280×800 開啟首頁 → 驗證標題 → 加入香蕉 | 標題正確、件數 1 件、總計 NT$ 20 | `test_ct02_compat_edge` |

---

## TestComplete 專案建議結構

```
Project
├── Script
│   └── web_shopping_cart_test_suite.py
├── NameMapping
│   └── Aliases（可選，見 web_shopping_cart_aliases.py）
└── TestItems
    ├── FT01_AddSingleItem      → test_ft01_add_single_item
    ├── FT02_AddMultipleItems   → test_ft02_add_multiple_items
    ├── FT03_IncreaseQuantity   → test_ft03_increase_quantity
    ├── FT04_RemoveItem         → test_ft04_remove_item
    ├── FT05_ClearCart          → test_ft05_clear_cart
    ├── FT06_CheckoutSuccess    → test_ft06_checkout_success
    ├── NT01_CheckoutEmptyCart  → test_nt01_checkout_empty_cart
    ├── NT02_DecreaseToZero     → test_nt02_decrease_to_zero
    ├── NT03_ClearEmptyCart     → test_nt03_clear_empty_cart
    ├── CT01_CompatChrome       → test_ct01_compat_chrome
    ├── CT02_CompatEdge         → test_ct02_compat_edge
    └── ALL_RunTestSuite        → TestMain
```

---

## 執行方式

```powershell
cd SQA_Transfer_Testcase\demo\shopping_cart
python serve.py
```

1. 在 TestComplete 建立 Web Testing 專案（Python）
2. 匯入 `web_shopping_cart_test_suite.py`
3. 執行 `TestMain()` 跑完全部 11 案例，或個別指向上表函式

---

## 通過 / 失敗準則

- **通過**：所有驗證點符合預期，Log 顯示 `passed`
- **失敗**：任一驗證不符，Log.Error 並截圖 `test_suite_error.png`
