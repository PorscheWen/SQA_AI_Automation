# -*- coding: utf-8 -*-
"""
TestComplete Python：Demo Shop 購物車完整測試套件（11 案例）

分類：
  - 黑箱功能測試 FT01–FT06（6）
  - 負面測試       NT01–NT03（3）
  - 相容測試       CT01 Chrome、CT02 Edge（2）

使用前：
  1. cd demo/shopping_cart && python serve.py
  2. 確認 http://localhost:8888/ 可開啟
  3. 複製本腳本至 TestComplete Script 單元，執行 TestMain()
"""

import Sys
from TestComplete import *

BASE_URL = "http://localhost:8888/"
PAGE_TITLE = "簡易購物車 - Demo Shop"
TIMEOUT_MS = 30000
VIEWPORT_WIDTH = 1280
VIEWPORT_HEIGHT = 800


# --- 共用工具 ---

def open_browser(url, browser_type=btChrome, timeout_ms=TIMEOUT_MS):
    """開啟指定瀏覽器並等待頁面載入。"""
    Browsers.Item[browser_type].Run(url)
    Sys.Browser("*").WaitPage("*", timeout_ms)
    set_viewport(VIEWPORT_WIDTH, VIEWPORT_HEIGHT)


def set_viewport(width, height):
    """設定瀏覽器視窗大小（相容測試用）。"""
    browser = Sys.Browser("*")
    browser.Width = width
    browser.Height = height


def close_browser():
    """關閉目前瀏覽器。"""
    try:
        Sys.Browser("*").Close()
    except Exception:
        pass


def get_page():
    return Sys.Browser("*").Page("*")


def find_by_testid(test_id, timeout=10):
    xpath = "//*[@data-testid='%s']" % test_id
    element = get_page().FindChildByXPath(xpath, timeout)
    if element is None:
        Log.Error("Element not found: data-testid=%s" % test_id)
    return element


def find_by_id(element_id, timeout=10):
    xpath = "//*[@id='%s']" % element_id
    element = get_page().FindChildByXPath(xpath, timeout)
    if element is None:
        Log.Error("Element not found: id=%s" % element_id)
    return element


def click_testid(test_id):
    element = find_by_testid(test_id)
    if element is not None:
        element.Click()
        get_page().Wait()


def get_text_by_testid(test_id):
    element = find_by_testid(test_id)
    if element is None:
        return ""
    return element.contentText


def verify_text(test_id, expected, label):
    actual = get_text_by_testid(test_id)
    Log.Message("%s: expected=%s, actual=%s" % (label, expected, actual))
    if actual != expected:
        Log.Error("%s mismatch: expected=%s, actual=%s" % (label, expected, actual))
        return False
    return True


def verify_page_title(expected_title=PAGE_TITLE):
    actual = get_page().title
    Log.Message("Page title: " + actual)
    if actual != expected_title:
        Log.Error("Title mismatch: expected=%s, actual=%s" % (expected_title, actual))
        return False
    return True


def verify_checkout_modal_closed(label="Checkout modal"):
    modal = find_by_id("checkout-modal")
    if modal is None:
        return False
    if modal.Visible:
        Log.Error("%s should be closed (not visible)" % label)
        return False
    Log.Message("%s is closed" % label)
    return True


def verify_checkout_modal_open(label="Checkout modal"):
    modal = find_by_id("checkout-modal")
    if modal is None:
        return False
    if not modal.Visible:
        Log.Error("%s should be open (visible)" % label)
        return False
    Log.Message("%s is open" % label)
    return True


def reset_cart():
    count_text = get_text_by_testid("cart-count")
    if count_text and count_text != "0 件":
        click_testid("clear-cart")
        get_page().Wait()


def take_screenshot(filename):
    get_page().Picture().SaveToFile(filename)
    Log.Message("Screenshot saved: " + filename)


def log_result(test_id, passed):
    if passed:
        Log.Message("%s passed" % test_id)
    else:
        Log.Error("%s failed" % test_id)
    return passed


# --- 黑箱功能測試 FT01–FT06 ---

def test_ft01_add_single_item():
    """FT01：加入單一商品。"""
    Log.Message("=== FT01: Add single item ===")
    reset_cart()
    click_testid("add-apple")

    passed = True
    passed = verify_text("cart-count", "1 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 30", "Cart total") and passed
    passed = verify_text("qty-value-apple", "1", "Apple quantity") and passed
    return log_result("FT01", passed)


def test_ft02_add_multiple_items():
    """FT02：加入多種商品，總計 NT$ 105。"""
    Log.Message("=== FT02: Add multiple items ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("add-banana")
    click_testid("add-milk")

    passed = True
    passed = verify_text("cart-count", "3 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 105", "Cart total") and passed
    return log_result("FT02", passed)


def test_ft03_increase_quantity():
    """FT03：使用 + 增加數量。"""
    Log.Message("=== FT03: Increase quantity ===")
    reset_cart()
    click_testid("add-banana")
    click_testid("qty-plus-banana")

    passed = True
    passed = verify_text("qty-value-banana", "2", "Banana quantity") and passed
    passed = verify_text("cart-count", "2 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 40", "Cart total") and passed
    return log_result("FT03", passed)


def test_ft04_remove_item():
    """FT04：移除商品。"""
    Log.Message("=== FT04: Remove item ===")
    reset_cart()
    click_testid("add-milk")
    click_testid("remove-milk")

    passed = True
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-empty", "購物車是空的，請從左側加入商品。", "Empty message") and passed
    return log_result("FT04", passed)


def test_ft05_clear_cart():
    """FT05：清空購物車。"""
    Log.Message("=== FT05: Clear cart ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("add-milk")
    click_testid("clear-cart")

    passed = True
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-total", "NT$ 0", "Cart total") and passed
    return log_result("FT05", passed)


def test_ft06_checkout_success():
    """FT06：結帳成功並關閉 Modal。"""
    Log.Message("=== FT06: Checkout success ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("add-banana")
    click_testid("checkout")

    passed = True
    passed = verify_checkout_modal_open() and passed
    passed = verify_text("checkout-message", "感謝您的購買！訂單已成立。", "Checkout message") and passed
    passed = verify_text("checkout-total", "總金額：NT$ 50", "Checkout total") and passed

    click_testid("close-modal")

    passed = verify_checkout_modal_closed() and passed
    passed = verify_text("cart-count", "0 件", "Cart count after checkout") and passed
    return log_result("FT06", passed)


# --- 負面測試 NT01–NT03 ---

def test_nt01_checkout_empty_cart():
    """NT01：空購物車按結帳，Modal 不應出現。"""
    Log.Message("=== NT01: Checkout with empty cart ===")
    reset_cart()
    click_testid("checkout")

    passed = True
    passed = verify_checkout_modal_closed() and passed
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-empty", "購物車是空的，請從左側加入商品。", "Empty message") and passed
    return log_result("NT01", passed)


def test_nt02_decrease_to_zero():
    """NT02：數量減至 0，商品自動移除。"""
    Log.Message("=== NT02: Decrease quantity to zero ===")
    reset_cart()
    click_testid("add-apple")
    click_testid("qty-minus-apple")

    passed = True
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-empty", "購物車是空的，請從左側加入商品。", "Empty message") and passed
    return log_result("NT02", passed)


def test_nt03_clear_empty_cart():
    """NT03：空購物車重複清空，狀態維持正常。"""
    Log.Message("=== NT03: Clear empty cart repeatedly ===")
    reset_cart()
    click_testid("clear-cart")
    click_testid("clear-cart")

    passed = True
    passed = verify_text("cart-count", "0 件", "Cart count") and passed
    passed = verify_text("cart-empty", "購物車是空的，請從左側加入商品。", "Empty message") and passed
    passed = verify_checkout_modal_closed() and passed
    return log_result("NT03", passed)


# --- 相容測試 CT01 ---

def _compat_smoke_test(browser_label):
    """相容測試共用：首頁標題 + 加入香蕉。"""
    passed = True
    passed = verify_page_title() and passed
    reset_cart()
    click_testid("add-banana")
    passed = verify_text("cart-count", "1 件", "%s cart count" % browser_label) and passed
    passed = verify_text("cart-total", "NT$ 20", "%s cart total" % browser_label) and passed
    return passed


def test_ct01_compat_chrome():
    """CT01：Chrome 瀏覽器相容。"""
    Log.Message("=== CT01: Chrome compatibility ===")
    close_browser()
    open_browser(BASE_URL, btChrome)
    passed = _compat_smoke_test("Chrome")
    close_browser()
    return log_result("CT01", passed)


def test_ct02_compat_edge():
    """CT02：Edge 瀏覽器相容。"""
    Log.Message("=== CT02: Edge compatibility ===")
    close_browser()
    open_browser(BASE_URL, btEdge)
    passed = _compat_smoke_test("Edge")
    close_browser()
    return log_result("CT02", passed)


# --- 執行入口 ---

ALL_TESTS = [
    test_ft01_add_single_item,
    test_ft02_add_multiple_items,
    test_ft03_increase_quantity,
    test_ft04_remove_item,
    test_ft05_clear_cart,
    test_ft06_checkout_success,
    test_nt01_checkout_empty_cart,
    test_nt02_decrease_to_zero,
    test_nt03_clear_empty_cart,
    test_ct01_compat_chrome,
    test_ct02_compat_edge,
]


def run_all_tests():
    """依序執行全部 11 案例。"""
    results = []

    open_browser(BASE_URL, btChrome)
    if not verify_page_title():
        close_browser()
        return False

    for test_fn in ALL_TESTS[:9]:
        results.append(test_fn())

    close_browser()

    results.append(test_ct01_compat_chrome())
    results.append(test_ct02_compat_edge())

    return all(results)


def TestMain():
    """TestComplete 入口：執行完整測試套件。"""
    try:
        if run_all_tests():
            Log.Message("Test suite passed: 11 cases (FT×6 + NT×3 + CT×2)")
        else:
            raise Exception("One or more test cases failed")
    except Exception as e:
        Log.Error("Test suite failed: " + str(e))
        try:
            take_screenshot("test_suite_error.png")
        except Exception:
            pass
        raise
    finally:
        close_browser()
