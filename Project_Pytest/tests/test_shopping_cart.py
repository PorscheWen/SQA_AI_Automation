"""
Shopping Cart Tests — TC01 to TC06
從 TestComplete Testcase_shopping_cart 直接轉換為 pytest。

對應關係：
  TestComplete                    pytest
  ─────────────────────────────   ──────────────────────────────────
  control_click_testid(id)     →  page._js_click(id)
  control_get_text_by_testid(id)→  page._js_text(id) / get_*(id)
  control_verify_text(...)     →  assert page.get_*() == expected
  control_reset_cart()         →  page.reset_cart()
  Log.Message / Log.Error      →  pytest assert / print

前置作業：
  cd demo/shopping_cart && python serve.py
  pytest tests/test_shopping_cart.py --headless
"""
import pytest
from page_objects.shopping_cart_page import ShoppingCartPage

PAGE_TITLE = "簡易購物車 - Demo Shop"


@pytest.fixture
def page(browser, base_url):
    """開啟購物車頁面，回傳頁面物件。"""
    browser.get(base_url)
    return ShoppingCartPage(browser)


class TestShoppingCart:
    """TC01–TC06：購物車核心功能（對應 TestComplete Testcase_shopping_cart）。"""

    def test_tc01_add_single_item(self, page):
        """TC01：加入單一商品，驗證件數與總計。"""
        page.reset_cart()
        page.click_add_apple()

        assert page.get_cart_count() == "1 件",  "TC01 件數應為 1 件"
        assert page.get_cart_total() == "NT$ 30", "TC01 總計應為 NT$ 30"
        assert page.get_qty_value("apple") == "1", "TC01 蘋果數量應為 1"

    def test_tc02_add_multiple_items(self, page):
        """TC02：加入多種商品，驗證總計 30+20+55=105。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_add_banana()
        page.click_add_milk()

        assert page.get_cart_count() == "3 件",   "TC02 件數應為 3 件"
        assert page.get_cart_total() == "NT$ 105", "TC02 總計應為 NT$ 105"

    def test_tc03_increase_quantity(self, page):
        """TC03：使用 + 按鈕增加數量，驗證總計。"""
        page.reset_cart()
        page.click_add_banana()
        page.click_qty_plus("banana")

        assert page.get_qty_value("banana") == "2", "TC03 香蕉數量應為 2"
        assert page.get_cart_count() == "2 件",     "TC03 件數應為 2 件"
        assert page.get_cart_total() == "NT$ 40",   "TC03 總計應為 NT$ 40"

    def test_tc04_remove_item(self, page):
        """TC04：移除商品後，購物車回到空狀態。"""
        page.reset_cart()
        page.click_add_milk()
        page.click_remove("milk")

        assert page.get_cart_count() == "0 件", "TC04 件數應為 0 件"
        assert page.get_cart_empty_message() == "購物車是空的，請從左側加入商品。", \
            "TC04 應顯示空購物車訊息"

    def test_tc05_clear_cart(self, page):
        """TC05：加入多項後清空購物車。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_add_milk()
        page.click_clear_cart()

        assert page.get_cart_count() == "0 件",  "TC05 件數應為 0 件"
        assert page.get_cart_total() == "NT$ 0", "TC05 總計應為 NT$ 0"

    def test_tc06_checkout(self, page):
        """TC06：結帳流程，驗證成功 Modal 與總金額。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_add_banana()
        page.click_checkout()

        assert page.get_checkout_message() == "感謝您的購買！訂單已成立。", \
            "TC06 結帳訊息不符"
        assert page.get_checkout_total() == "總金額：NT$ 50", \
            "TC06 總金額應為 NT$ 50"

        page.click_close_modal()
