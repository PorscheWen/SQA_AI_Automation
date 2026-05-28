"""
Shopping Cart Full Test Suite — FT01-FT06, NT01-NT03, CT01-CT02
從 TestComplete web_shopping_cart_test_suite.py 直接轉換為 pytest。

分類：
  - 黑箱功能測試  FT01–FT06  (6)  @pytest.mark.functional
  - 負面測試      NT01–NT03  (3)  @pytest.mark.negative
  - 相容測試      CT01–CT02  (2)  @pytest.mark.compat

前置作業：
  cd demo/shopping_cart && python serve.py
  pytest tests/test_full_suite.py --headless
"""
import os
import shutil
import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options as ChromeOptions
from page_objects.shopping_cart_page import ShoppingCartPage

PAGE_TITLE = "簡易購物車 - Demo Shop"


@pytest.fixture
def page(browser, base_url):
    """開啟購物車頁面，回傳頁面物件。"""
    browser.get(base_url)
    return ShoppingCartPage(browser)


# ===========================================================================
# 黑箱功能測試 FT01–FT06
# ===========================================================================

@pytest.mark.functional
class TestFunctional:
    """FT01–FT06：對應 TestComplete testcase_ft0{1-6}_*。"""

    def test_ft01_add_single_item(self, page):
        """FT01：加入單一商品。"""
        page.reset_cart()
        page.click_add_apple()

        assert page.get_cart_count() == "1 件"
        assert page.get_cart_total() == "NT$ 30"
        assert page.get_qty_value("apple") == "1"

    def test_ft02_add_multiple_items(self, page):
        """FT02：加入多種商品，總計 NT$ 105（30+20+55）。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_add_banana()
        page.click_add_milk()

        assert page.get_cart_count() == "3 件"
        assert page.get_cart_total() == "NT$ 105"

    def test_ft03_increase_quantity(self, page):
        """FT03：使用 + 增加數量，驗證小計。"""
        page.reset_cart()
        page.click_add_banana()
        page.click_qty_plus("banana")

        assert page.get_qty_value("banana") == "2"
        assert page.get_cart_count() == "2 件"
        assert page.get_cart_total() == "NT$ 40"

    def test_ft04_remove_item(self, page):
        """FT04：移除商品，購物車回到空狀態。"""
        page.reset_cart()
        page.click_add_milk()
        page.click_remove("milk")

        assert page.get_cart_count() == "0 件"
        assert page.get_cart_empty_message() == "購物車是空的，請從左側加入商品。"

    def test_ft05_clear_cart(self, page):
        """FT05：清空購物車。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_add_milk()
        page.click_clear_cart()

        assert page.get_cart_count() == "0 件"
        assert page.get_cart_total() == "NT$ 0"

    def test_ft06_checkout_success(self, page):
        """FT06：結帳成功並關閉 Modal；結帳後購物車自動清空。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_add_banana()
        page.click_checkout()

        assert page.is_checkout_modal_open(), "FT06：結帳 Modal 應已開啟"
        assert page.get_checkout_message() == "感謝您的購買！訂單已成立。"
        assert page.get_checkout_total() == "總金額：NT$ 50"

        page.click_close_modal()

        assert not page.is_checkout_modal_open(), "FT06：關閉後 Modal 應已隱藏"
        assert page.get_cart_count() == "0 件", "FT06：結帳後購物車應自動清空"


# ===========================================================================
# 負面測試 NT01–NT03
# ===========================================================================

@pytest.mark.negative
class TestNegative:
    """NT01–NT03：邊界條件與錯誤情境。"""

    def test_nt01_checkout_empty_cart(self, page):
        """
        NT01：空購物車按結帳，Modal 不應出現。

        說明：app.js 在 getTotalQuantity() == 0 時直接 return，不開啟 Modal。
              按鈕此時被父容器 hidden 屬性隱藏，以 JS 觸發仍安全。
        """
        page.reset_cart()
        page.click_checkout()  # JS click；JS guard 攔截，Modal 不開啟

        assert not page.is_checkout_modal_open(), "NT01：空購物車不應開啟結帳 Modal"
        assert page.get_cart_count() == "0 件"
        assert page.get_cart_empty_message() == "購物車是空的，請從左側加入商品。"

    def test_nt02_decrease_to_zero(self, page):
        """NT02：數量減至 0，商品自動移除，購物車回空。"""
        page.reset_cart()
        page.click_add_apple()
        page.click_qty_minus("apple")  # qty 0 → item deleted

        assert page.get_cart_count() == "0 件"
        assert page.get_cart_empty_message() == "購物車是空的，請從左側加入商品。"

    def test_nt03_clear_empty_cart(self, page):
        """NT03：空購物車重複清空，狀態維持正常（不崩潰）。"""
        page.reset_cart()
        page.click_clear_cart()  # 購物車已空，按鈕隱藏，JS click 觸發
        page.click_clear_cart()  # 再次清空

        assert page.get_cart_count() == "0 件"
        assert page.get_cart_empty_message() == "購物車是空的，請從左側加入商品。"
        assert not page.is_checkout_modal_open()


# ===========================================================================
# 相容測試 CT01–CT02
# ===========================================================================

@pytest.mark.compat
class TestCompatibility:
    """CT01–CT02：瀏覽器相容性煙霧測試。"""

    def test_ct01_compat_chrome(self, page):
        """
        CT01：Chrome 瀏覽器煙霧測試。

        說明：browser fixture 預設即為 Chrome，此測試驗證頁面標題
              與基本購物功能，確認 Chrome 環境正常運作。
        """
        assert page.get_page_title() == PAGE_TITLE, "CT01：頁面標題不符"
        page.reset_cart()
        page.click_add_banana()

        assert page.get_cart_count() == "1 件",  "CT01：件數應為 1 件"
        assert page.get_cart_total() == "NT$ 20", "CT01：總計應為 NT$ 20"

    @pytest.mark.skipif(
        not any(
            os.path.exists(p) for p in (
                "/usr/bin/microsoft-edge",
                "/usr/bin/microsoft-edge-stable",
                "/opt/microsoft/msedge/msedge",
            )
        ),
        reason="Microsoft Edge 未安裝（僅 Windows/特定 Linux 環境適用）",
    )
    def test_ct02_compat_edge(self, base_url):
        """
        CT02：Edge 瀏覽器煙霧測試。

        說明：單獨建立 Edge WebDriver 執行個體，測試完成後關閉。
              若 Edge 未安裝則自動略過。
        """
        from selenium.webdriver.edge.options import Options as EdgeOptions
        from selenium.webdriver.edge.service import Service as EdgeService

        opts = EdgeOptions()
        opts.add_argument("--no-sandbox")
        opts.add_argument("--disable-dev-shm-usage")
        opts.add_argument("--window-size=1280,800")

        try:
            from webdriver_manager.microsoft import EdgeChromiumDriverManager
            service = EdgeService(EdgeChromiumDriverManager().install())
            driver = webdriver.Edge(service=service, options=opts)
        except Exception:
            driver = webdriver.Edge(options=opts)

        driver.implicitly_wait(10)
        try:
            driver.get(base_url)
            page = ShoppingCartPage(driver)

            assert page.get_page_title() == PAGE_TITLE, "CT02：頁面標題不符"
            page.reset_cart()
            page.click_add_banana()

            assert page.get_cart_count() == "1 件",  "CT02：件數應為 1 件"
            assert page.get_cart_total() == "NT$ 20", "CT02：總計應為 NT$ 20"
        finally:
            driver.quit()
