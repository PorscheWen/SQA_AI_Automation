# -*- coding: utf-8 -*-
"""
TestComplete Python 範例：購物車（Name Mapping 版）

用途：示範以 Aliases 操作購物車，適合 UI 常改、需長期維護的專案。
搭配 demo/shopping_cart 示範頁使用。

Name Mapping 結構（請在 TestComplete 手動建立或錄製後調整）：
    Aliases.browser.pageShop
        ├── btnAddApple          → #btn-add-apple
        ├── btnAddBanana         → #btn-add-banana
        ├── btnAddMilk           → #btn-add-milk
        ├── labelCartCount       → #cart-count
        ├── labelCartTotal       → #cart-total
        ├── labelCartEmpty       → #cart-empty-message
        ├── btnQtyPlusBanana     → #btn-qty-plus-banana
        ├── labelQtyBanana       → #qty-value-banana
        ├── btnRemoveMilk        → #btn-remove-milk
        ├── btnClearCart         → #btn-clear-cart
        ├── btnCheckout          → #btn-checkout
        ├── labelCheckoutMessage → #checkout-message
        ├── labelCheckoutTotal   → #checkout-total
        └── btnCloseModal        → #btn-close-modal
"""

import Sys
from TestComplete import *

BASE_URL = "http://localhost:8888/"


def control_open_browser(url, timeout_ms=30000):
    Browsers.Item[btChrome].Run(url)
    Sys.Browser("*").WaitPage("*", timeout_ms)


def control_get_shop_page():
    return Aliases.browser.pageShop


def control_control_reset_cart():
    page = control_get_shop_page()
    if page.labelCartCount.contentText != "0 件":
        page.btnClearCart.Click()
        page.Wait()


def control_verify_alias_text(alias_obj, expected, label):
    actual = alias_obj.contentText
    Log.Message("%s: expected=%s, actual=%s" % (label, expected, actual))
    aqObject.CheckProperty(alias_obj, "contentText", cmpEqual, expected)


def testcase_add_single_item_aliases():
    """TC01（Aliases）：加入蘋果。"""
    page = control_get_shop_page()
    control_reset_cart()
    page.btnAddApple.Click()
    page.Wait()

    control_verify_alias_text(page.labelCartCount, "1 件", "Cart count")
    control_verify_alias_text(page.labelCartTotal, "NT$ 30", "Cart total")


def testcasecase_checkout_aliases():
    """TC06（Aliases）：結帳流程。"""
    page = control_get_shop_page()
    control_reset_cart()
    page.btnAddApple.Click()
    page.btnAddBanana.Click()
    page.btnCheckout.Click()

    control_verify_alias_text(page.labelCheckoutMessage, "感謝您的購買！訂單已成立。", "Checkout message")
    control_verify_alias_text(page.labelCheckoutTotal, "總金額：NT$ 50", "Checkout total")
    page.btnCloseModal.Click()


def TestMain():
    """TestComplete 入口函式（Name Mapping 版）。"""
    try:
        control_open_browser(BASE_URL)
        testcase_add_single_item_aliases()
        testcase_checkout_aliases()
        Log.Message("Alias-based shopping cart tests passed")
    except Exception as e:
        Log.Error("Test failed: " + str(e))
        Sys.Browser("*").Page("*").Picture().SaveToFile("shopping_cart_alias_error.png")
        raise
    finally:
        try:
            Aliases.browser.Close()
        except Exception:
            pass
