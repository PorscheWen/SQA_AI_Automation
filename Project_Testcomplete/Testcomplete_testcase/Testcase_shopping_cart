import Sys
from TestComplete import *


# ===== 統一的控制函數 =====

def control_element(action, test_id=None, expected=None, label=None, timeout=10):
    """
    統一的元素控制函數，整合元素相關操作。
    
    參數:
        action: 動作類型 - "find", "click", "get_text", "verify_text"
        test_id: data-testid 屬性值
        expected: 預期文字（用於 verify_text）
        label: 標籤名稱（用於 verify_text）
        timeout: 尋找元素的超時時間（秒）
    
    使用範例:
        control_element("find", test_id="add-apple")
        control_element("click", test_id="add-apple")
        control_element("get_text", test_id="cart-count")
        control_element("verify_text", test_id="cart-count", expected="1 件", label="Cart count")
    """
    if action == "find":
        # 以 data-testid 屬性找到元素
        xpath = "//*[@data-testid='%s']" % test_id
        element = control_page("get").FindChildByXPath(xpath, timeout)
        if element is None:
            Log.Error("Element not found: data-testid=%s" % test_id)
        return element
    
    elif action == "click":
        # 點擊指定 data-testid 的元素
        element = control_element("find", test_id=test_id, timeout=timeout)
        if element is not None:
            element.Click()
            control_page("wait")
        return element
    
    elif action == "get_text":
        # 讀取指定 data-testid 元素的文字
        element = control_element("find", test_id=test_id, timeout=timeout)
        if element is None:
            return ""
        return element.contentText
    
    elif action == "verify_text":
        # 驗證元素文字是否符合預期
        actual = control_element("get_text", test_id=test_id, timeout=timeout)
        Log.Message("%s: expected=%s, actual=%s" % (label, expected, actual))
        if actual != expected:
            Log.Error("%s mismatch: expected=%s, actual=%s" % (label, expected, actual))
            return False
        return True
    
    else:
        Log.Error("Unknown action: %s" % action)
        return None


def control_page(action, value=None, timeout_ms=None):
    """
    統一的頁面控制函數，整合頁面相關操作。
    
    參數:
        action: 動作類型 - "get", "open", "verify_title", "screenshot", "wait"
        value: 根據不同 action 有不同含義：
               - "open": URL 字串
               - "verify_title": 預期標題字串
               - "screenshot": 檔案名稱字串
        timeout_ms: 超時時間（毫秒，用於 open）
    
    使用範例:
        control_page("get")
        control_page("open", value="http://example.com", timeout_ms=30000)
        control_page("verify_title", value="Shopping Cart")
        control_page("screenshot", value="error.png")
        control_page("wait")
    """
    if action == "get":
        # 取得目前頁面物件
        return Sys.Browser("*").Page("*")
    
    elif action == "open":
        # 開啟 Chrome 並等待頁面載入完成
        if timeout_ms is None:
            timeout_ms = TIMEOUT_MS
        Browsers.Item[btChrome].Run(value)
        Sys.Browser("*").WaitPage("*", timeout_ms)
    
    elif action == "verify_title":
        # 驗證頁面標題
        actual = control_page("get").title
        Log.Message("Page title: " + actual)
        if actual != value:
            Log.Error("Title mismatch: expected=%s, actual=%s" % (value, actual))
            return False
        return True
    
    elif action == "screenshot":
        # 失敗或除錯時截圖
        control_page("get").Picture().SaveToFile(value)
        Log.Message("Screenshot saved: " + value)
    
    elif action == "wait":
        # 等待頁面載入
        control_page("get").Wait()
    
    else:
        Log.Error("Unknown action: %s" % action)
        return None


def control_cart(action):
    """
    統一的購物車控制函數，處理購物車特定操作。
    
    參數:
        action: 動作類型 - "reset"
    
    使用範例:
        control_cart("reset")
    """
    if action == "reset":
        # 若購物車有商品，先清空以確保測試獨立
        count_text = control_element("get_text", test_id="cart-count")
        if count_text and count_text != "0 件":
            control_element("click", test_id="clear-cart")
            control_page("wait")
    else:
        Log.Error("Unknown action: %s" % action)


# ===== 舊版相容函數（保留以便向後相容）=====

def control_open_browser(url, timeout_ms=TIMEOUT_MS):
    """開啟 Chrome 並等待頁面載入完成。"""
    control_page("open", value=url, timeout_ms=timeout_ms)


def control_get_page():
    """取得目前頁面物件。"""
    return control_page("get")


def control_find_by_testid(test_id, timeout=10):
    """以 data-testid 屬性找到元素。"""
    return control_element("find", test_id=test_id, timeout=timeout)


def control_click_testid(test_id):
    """點擊指定 data-testid 的元素。"""
    control_element("click", test_id=test_id)


def control_get_text_by_testid(test_id):
    """讀取指定 data-testid 元素的文字。"""
    return control_element("get_text", test_id=test_id)


def control_verify_text(test_id, expected, label):
    """驗證元素文字是否符合預期。"""
    return control_element("verify_text", test_id=test_id, expected=expected, label=label)


def control_verify_page_title(expected_title):
    """驗證頁面標題。"""
    return control_page("verify_title", value=expected_title)


def control_reset_cart():
    """若購物車有商品，先清空以確保測試獨立。"""
    control_cart("reset")


def control_take_screenshot(filename):
    """失敗或除錯時截圖。"""
    control_page("screenshot", value=filename)


##### --- 測試案例 ---   ###########

def testcase_add_single_item():
    """TC01：加入單一商品，驗證件數與總計。"""
    Log.Message("=== TC01: Add single item ===")
    control_reset_cart()
    control_click_testid("add-apple")

    passed = True
    passed = control_verify_text("cart-count", "1 件", "Cart count") and passed
    passed = control_verify_text("cart-total", "NT$ 30", "Cart total") and passed
    passed = control_verify_text("qty-value-apple", "1", "Apple quantity") and passed

    if passed:
        Log.Message("TC01 passed")
    else:
        Log.Error("TC01 failed")
    return passed


def testcase_add_multiple_items():
    """TC02：加入多種商品，驗證總計 30+20+55=105。"""
    Log.Message("=== TC02: Add multiple items ===")
    control_reset_cart()
    control_click_testid("add-apple")
    control_click_testid("add-banana")
    control_click_testid("add-milk")

    passed = True
    passed = control_verify_text("cart-count", "3 件", "Cart count") and passed
    passed = control_verify_text("cart-total", "NT$ 105", "Cart total") and passed

    if passed:
        Log.Message("TC02 passed")
    else:
        Log.Error("TC02 failed")
    return passed


def testcase_increase_quantity():
    """TC03：使用 + 按鈕增加數量，驗證總計。"""
    Log.Message("=== TC03: Increase quantity ===")
    control_reset_cart()
    control_click_testid("add-banana")
    control_click_testid("qty-plus-banana")

    passed = True
    passed = control_verify_text("qty-value-banana", "2", "Banana quantity") and passed
    passed = control_verify_text("cart-count", "2 件", "Cart count") and passed
    passed = control_verify_text("cart-total", "NT$ 40", "Cart total") and passed

    if passed:
        Log.Message("TC03 passed")
    else:
        Log.Error("TC03 failed")
    return passed


def testcase_remove_item():
    """TC04：移除商品後，購物車回到空狀態。"""
    Log.Message("=== TC04: Remove item ===")
    control_reset_cart()
    control_click_testid("add-milk")
    control_click_testid("remove-milk")

    passed = True
    passed = control_verify_text("cart-count", "0 件", "Cart count") and passed
    passed = control_verify_text("cart-empty", "購物車是空的，請從左側加入商品。", "Empty message") and passed

    if passed:
        Log.Message("TC04 passed")
    else:
        Log.Error("TC04 failed")
    return passed


def testcase_clear_cart():
    """TC05：加入多項後清空購物車。"""
    Log.Message("=== TC05: Clear cart ===")
    control_reset_cart()
    control_click_testid("add-apple")
    control_click_testid("add-milk")
    control_click_testid("clear-cart")

    passed = True
    passed = control_verify_text("cart-count", "0 件", "Cart count") and passed
    passed = control_verify_text("cart-total", "NT$ 0", "Cart total") and passed

    if passed:
        Log.Message("TC05 passed")
    else:
        Log.Error("TC05 failed")
    return passed


def testcase_checkout():
    """TC06：結帳流程，驗證成功 Modal 與總金額。"""
    Log.Message("=== TC06: Checkout ===")
    control_reset_cart()
    control_click_testid("add-apple")
    control_click_testid("add-banana")
    control_click_testid("checkout")

    passed = True
    passed = control_verify_text("checkout-message", "感謝您的購買！訂單已成立。", "Checkout message") and passed
    passed = control_verify_text("checkout-total", "總金額：NT$ 50", "Checkout total") and passed

    control_click_testid("close-modal")

    if passed:
        Log.Message("TC06 passed")
    else:
        Log.Error("TC06 failed")
    return passed


def control_run_all_tests():
    """依序執行全部購物車測試案例。"""
    results = []
    results.append(testcase_add_single_item())
    results.append(testcase_add_multiple_items())
    results.append(testcase_increase_quantity())
    results.append(testcase_remove_item())
    results.append(testcase_clear_cart())
    results.append(testcase_checkout())
    return all(results)