"""
Step Definitions for Shopping Cart BDD Tests
購物車 BDD 測試的步驟定義
"""
from pytest_bdd import given, when, then, parsers, scenarios
from page_objects.product_list_page import ProductListPage
from page_objects.shopping_cart_page import ShoppingCartPage

# 載入所有場景
scenarios('../features/shopping_cart.feature')


# ==================== Given Steps ====================

@given('我已開啟購物車網頁', target_fixture='pages')
def open_shopping_cart_page(browser, base_url):
    """開啟購物車網頁"""
    browser.get(base_url)
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    return {
        'product_page': product_page,
        'cart_page': cart_page
    }


@given('購物車已清空')
def clear_cart(pages):
    """清空購物車"""
    cart_page = pages['cart_page']
    cart_page.clear_cart()


# ==================== When Steps ====================

@when('我點擊加入蘋果按鈕')
def click_add_apple(pages):
    """點擊加入蘋果按鈕"""
    product_page = pages['product_page']
    product_page.add_apple()


@when('我點擊加入香蕉按鈕')
def click_add_banana(pages):
    """點擊加入香蕉按鈕"""
    product_page = pages['product_page']
    product_page.add_banana()


@when('我點擊加入牛奶按鈕')
def click_add_milk(pages):
    """點擊加入牛奶按鈕"""
    product_page = pages['product_page']
    product_page.add_milk()


@when(parsers.parse('我點擊加入{商品}按鈕'))
def click_add_product(pages, 商品):
    """點擊加入商品按鈕（參數化）"""
    product_page = pages['product_page']
    product_page.add_product(商品)


@when('我點擊蘋果的增加數量按鈕')
def click_increase_apple_quantity(pages):
    """點擊蘋果的增加數量按鈕"""
    cart_page = pages['cart_page']
    cart_page.click_increase_quantity("蘋果")


@when(parsers.parse('我點擊{商品}的增加數量按鈕'))
def click_increase_quantity(pages, 商品):
    """點擊商品的增加數量按鈕（參數化）"""
    cart_page = pages['cart_page']
    cart_page.click_increase_quantity(商品)


@when('我點擊蘋果的移除按鈕')
def click_remove_apple(pages):
    """點擊蘋果的移除按鈕"""
    cart_page = pages['cart_page']
    cart_page.click_remove_product("蘋果")


@when(parsers.parse('我點擊{商品}的移除按鈕'))
def click_remove_product(pages, 商品):
    """點擊商品的移除按鈕（參數化）"""
    cart_page = pages['cart_page']
    cart_page.click_remove_product(商品)


@when('我點擊清空購物車按鈕')
def click_clear_cart(pages):
    """點擊清空購物車按鈕"""
    cart_page = pages['cart_page']
    cart_page.click_clear_cart()


@when('我點擊結帳按鈕')
def click_checkout(pages):
    """點擊結帳按鈕"""
    cart_page = pages['cart_page']
    cart_page.click_checkout()


# ==================== Then Steps ====================

@then(parsers.parse('購物車件數應該是 "{expected_count}"'))
def verify_cart_count(pages, expected_count):
    """驗證購物車件數"""
    cart_page = pages['cart_page']
    actual_count = cart_page.get_cart_count()
    assert actual_count == expected_count, \
        f"購物車件數不符: 預期 '{expected_count}', 實際 '{actual_count}'"


@then(parsers.parse('購物車總計應該是 "{expected_total}"'))
def verify_cart_total(pages, expected_total):
    """驗證購物車總計"""
    cart_page = pages['cart_page']
    actual_total = cart_page.get_cart_total()
    assert actual_total == expected_total, \
        f"購物車總計不符: 預期 '{expected_total}', 實際 '{actual_total}'"


@then(parsers.parse('蘋果的數量應該是 "{expected_quantity}"'))
def verify_apple_quantity(pages, expected_quantity):
    """驗證蘋果的數量"""
    cart_page = pages['cart_page']
    actual_quantity = cart_page.get_product_quantity("蘋果")
    assert actual_quantity == expected_quantity, \
        f"蘋果數量不符: 預期 '{expected_quantity}', 實際 '{actual_quantity}'"


@then(parsers.parse('{商品}的數量應該是 "{expected_quantity}"'))
def verify_product_quantity(pages, 商品, expected_quantity):
    """驗證商品的數量（參數化）"""
    cart_page = pages['cart_page']
    actual_quantity = cart_page.get_product_quantity(商品)
    assert actual_quantity == expected_quantity, \
        f"{商品}數量不符: 預期 '{expected_quantity}', 實際 '{actual_quantity}'"


@then('應該顯示結帳確認訊息')
def verify_checkout_message(pages):
    """驗證結帳確認訊息"""
    cart_page = pages['cart_page']
    assert cart_page.is_checkout_message_visible(), \
        "結帳確認訊息未顯示"
    message = cart_page.get_checkout_message()
    assert len(message) > 0, "結帳訊息為空"
