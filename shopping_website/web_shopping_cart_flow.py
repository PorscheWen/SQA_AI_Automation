# -*- coding: utf-8 -*-
"""
TestComplete Python 範例：購物車流程（精簡版，6 案例）

完整 10 案例測試套件（含負面 / 相容測試）請改用：
  web_shopping_cart_test_suite.py
  TEST_PLAN.md
"""
import Sys
from TestComplete import *

# --- 可調整設定 ---
BASE_URL = "http://localhost:8888/"
PAGE_TITLE = "簡易購物車 - Demo Shop"
TIMEOUT_MS = 30000


def open_browser(url, timeout_ms=TIMEOUT_MS):
    """開啟 Chrome 並等待頁面載入完成。"""
    Browsers.Item[btChrome].Run(url)
    Sys.Browser("*").WaitPage("*", timeout_ms)


def get_page():
    """取得目前頁面物件。"""
    return Sys.Browser("*").Page("*")


def find_by_testid(test_id, timeout=10):
    """以 data-testid 屬性找到元素。"""
    xpath = "//*[@data-testid='%s']" % test_id
    element = get_page().FindChildByXPath(xpath, timeout)
    if element is None:
        Log.Error("Element not found: data-testid=%s" % test_id)
    return element


def click_testid(test_id):
    """點擊指定 data-testid 的元素。"""
    element = find_by_testid(test_id)
    if element is not None:
        element.Click()
        get_page().Wait()


def get_text_by_testid(test_id):
    """讀取指定 data-testid 元素的文字。"""
    element = find_by_testid(test_id)
    if element is None:
        return ""
    return element.contentText


def verify_text(test_id, expected, label):
    """驗證元素文字是否符合預期。"""
    actual = get_text_by_testid(test_id)
    Log.Message("%s: expected=%s, actual=%s" % (label, expected, actual))
    if actual != expected:
        Log.Error("%s mismatch: expected=%s, actual=%s" % (label, expected, actual))
        return False
    return True


def verify_page_title(expected_title):
    """驗證頁面標題。"""
    actual = get_page().title
    Log.Message("Page title: " + actual)
    if actual != expected_title:
        Log.Error("Title mismatch: expected=%s, actual=%s" % (expected_title, actual))
        return False
    return True


def reset_cart():
    """若購物車有商品，先清空以確保測試獨立。"""
    count_text = get_text_by_testid("cart-count")
    if count_text and count_text != "0 件":
        click_testid("clear-cart")
        get_page().Wait()


def take_screenshot(filename):
    """失敗或除錯時截圖。"""
    get_page().Picture().SaveToFile(filename)
    Log.Message("Screenshot saved: " + filename)


# --- 測試案例 ---

def test_add_single_item():
    """TC01：加入單一商品，驗證件數與總計。"""
    Log.Message("=== TC01: Add single item ===")
    reset_cart()
    click_testid("add-apple")

    passed = True
    passed = verify_text("cart-count", "1 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 30", "Cart total") and passed
    passed = verify_text("qty-value-apple", "1", "Apple quantity") and passed

    if passed:
        Log.Message("TC01 passed")
    else:
        Log.Error("TC01 failed")
    return passed


def test_add_multiple_items():
    """TC02：加入多種商品，驗證總計 30+20+55=105。"""
    Log.Message("=== TC02: Add multiple items ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("add-banana")
    click_testid("add-milk")

    passed = True
    passed = verify_text("cart-count", "3 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 105", "Cart total") and passed

    if passed:
        Log.Message("TC02 passed")
    else:
        Log.Error("TC02 failed")
    return passed


def test_increase_quantity():
    """TC03：使用 + 按鈕增加數量，驗證總計。"""
    Log.Message("=== TC03: Increase quantity ===")
    reset_cart()
    click_testid("add-banana")
    click_testid("qty-plus-banana")

    passed = True
    passed = verify_text("qty-value-banana", "2", "Banana quantity") and passed
    passed = verify_text("cart-count", "2 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 40", "Cart total") and passed

    if passed:
        Log.Message("TC03 passed")
    else:
        Log.Error("TC03 failed")
    return passed


def test_remove_item():
    """TC04：移除商品後，購物車回到空狀態。"""
    Log.Message("=== TC04: Remove item ===")
    reset_cart()
    click_testid("add-milk")
    click_testid("remove-milk")

    passed = True
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-empty", "購物車是空的，請從左側加入商品。", "Empty message") and passed

    if passed:
        Log.Message("TC04 passed")
    else:
        Log.Error("TC04 failed")
    return passed


def test_clear_cart():
    """TC05：加入多項後清空購物車。"""
    Log.Message("=== TC05: Clear cart ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("add-milk")
    click_testid("clear-cart")

    passed = True
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 0", "Cart total") and passed

    if passed:
        Log.Message("TC05 passed")
    else:
        Log.Error("TC05 failed")
    return passed


def test_checkout():
    """TC06：結帳流程，驗證成功 Modal 與總金額。"""
    Log.Message("=== TC06: Checkout ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("add-banana")
    click_testid("checkout")

    passed = True
    passed = verify_text("checkout-message", "感謝您的購買！訂單已成立。", "Checkout message") and passed
    passed = verify_text("checkout-total", "總金額：NT$ 50", "Checkout total") and passed

    click_testid("close-modal")

    if passed:
        Log.Message("TC06 passed")
    else:
        Log.Error("TC06 failed")
    return passed


def run_all_tests():
    """依序執行全部購物車測試案例。"""
    results = []
    results.append(test_add_single_item())
    results.append(test_add_multiple_items())
    results.append(test_increase_quantity())
    results.append(test_remove_item())
    results.append(test_clear_cart())
    results.append(test_checkout())
    return all(results)


def TestMain():
    """TestComplete 入口函式：執行全部購物車測試。"""
    try:
        open_browser(BASE_URL)
        if not verify_page_title(PAGE_TITLE):
            raise Exception("Page title verification failed")

        if run_all_tests():
            Log.Message("All shopping cart tests passed")
        else:
            raise Exception("One or more shopping cart tests failed")
    except Exception as e:
        Log.Error("Test failed: " + str(e))
        take_screenshot("shopping_cart_error.png")
        raise
    finally:
        try:
            Sys.Browser("*").Close()
        except Exception:
            pass
