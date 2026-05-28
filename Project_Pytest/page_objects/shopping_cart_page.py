"""
ShoppingCartPage：購物車示範應用的統一頁面物件。

設計原則：
  - 全部以 data-testid 屬性定位元素，與 TestComplete 腳本保持一致。
  - 使用 JavaScript 執行點擊與文字讀取，確保對隱藏元素（hidden attribute /
    display:none）的行為與 TestComplete 的 Click() / contentText 一致。
"""
from selenium.webdriver.common.by import By
from page_objects.base_page import BasePage


class ShoppingCartPage(BasePage):
    """購物車應用統一頁面物件（商品列表 + 購物車 + 結帳 Modal）。"""

    # checkout modal 以 id 定位（無 data-testid）
    _CHECKOUT_MODAL_ID = "checkout-modal"

    # ------------------------------------------------------------------
    # 私有 JS 工具方法
    # ------------------------------------------------------------------

    def _js_text(self, testid: str) -> str:
        """
        以 JS 讀取 data-testid 元素的 textContent（對隱藏元素也有效）。
        對應 TestComplete 的 element.contentText。
        """
        return self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"" + testid + "\"]');"
            "return el ? el.textContent.trim() : '';"
        )

    def _js_click(self, testid: str) -> None:
        """
        以 JS 點擊 data-testid 元素（對隱藏元素也有效）。
        對應 TestComplete 的 element.Click()。
        """
        self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"" + testid + "\"]');"
            "if (el) { el.click(); }"
        )

    # ------------------------------------------------------------------
    # 商品列表操作
    # ------------------------------------------------------------------

    def click_add_apple(self) -> None:
        """加入蘋果（add-apple）。"""
        self._js_click("add-apple")

    def click_add_banana(self) -> None:
        """加入香蕉（add-banana）。"""
        self._js_click("add-banana")

    def click_add_milk(self) -> None:
        """加入鮮奶（add-milk）。"""
        self._js_click("add-milk")

    def click_add_product(self, product_id: str) -> None:
        """
        以商品 ID 加入商品（apple / banana / milk）。
        對應 TestComplete 的 control_click_testid("add-{product_id}")。
        """
        self._js_click(f"add-{product_id}")

    # ------------------------------------------------------------------
    # 購物車項目操作（動態元素，依商品 ID）
    # ------------------------------------------------------------------

    def click_qty_plus(self, product_id: str) -> None:
        """點擊 +（增加數量）按鈕。對應 TestComplete qty-plus-{product_id}。"""
        self._js_click(f"qty-plus-{product_id}")

    def click_qty_minus(self, product_id: str) -> None:
        """點擊 -（減少數量）按鈕。對應 TestComplete qty-minus-{product_id}。"""
        self._js_click(f"qty-minus-{product_id}")

    def click_remove(self, product_id: str) -> None:
        """點擊移除按鈕。對應 TestComplete remove-{product_id}。"""
        self._js_click(f"remove-{product_id}")

    def get_qty_value(self, product_id: str) -> str:
        """讀取商品數量顯示值。對應 TestComplete qty-value-{product_id}。"""
        return self._js_text(f"qty-value-{product_id}")

    # ------------------------------------------------------------------
    # 購物車摘要
    # ------------------------------------------------------------------

    def get_cart_count(self) -> str:
        """購物車件數文字，例如 '1 件'。對應 TestComplete cart-count。"""
        return self._js_text("cart-count")

    def get_cart_total(self) -> str:
        """購物車總計文字，例如 'NT$ 30'。對應 TestComplete cart-total。"""
        return self._js_text("cart-total")

    def get_cart_empty_message(self) -> str:
        """空購物車提示文字。對應 TestComplete cart-empty。"""
        return self._js_text("cart-empty")

    # ------------------------------------------------------------------
    # 購物車動作按鈕（在購物車空時這些按鈕的父容器有 hidden 屬性）
    # ------------------------------------------------------------------

    def click_clear_cart(self) -> None:
        """清空購物車按鈕（購物車空時按鈕隱藏，仍以 JS 觸發）。"""
        self._js_click("clear-cart")

    def click_checkout(self) -> None:
        """結帳按鈕（購物車空時按鈕隱藏，仍以 JS 觸發）。"""
        self._js_click("checkout")

    def reset_cart(self) -> None:
        """
        若購物車非空則點清空，確保各測試案例獨立執行。
        對應 TestComplete control_reset_cart()。
        """
        if self.get_cart_count() != "0 件":
            self.click_clear_cart()

    # ------------------------------------------------------------------
    # 結帳 Modal
    # ------------------------------------------------------------------

    def is_checkout_modal_open(self) -> bool:
        """
        回傳 True 若結帳 Modal 目前為開啟狀態。
        以 CSS class 'is-open' 判斷（對應 app.js openCheckoutModal/closeCheckoutModal）。
        """
        modal = self.driver.find_element(By.ID, self._CHECKOUT_MODAL_ID)
        return "is-open" in (modal.get_attribute("class") or "")

    def get_checkout_message(self) -> str:
        """結帳成功訊息文字。對應 TestComplete checkout-message。"""
        return self._js_text("checkout-message")

    def get_checkout_total(self) -> str:
        """結帳總金額文字，例如 '總金額：NT$ 50'。對應 TestComplete checkout-total。"""
        return self._js_text("checkout-total")

    def click_close_modal(self) -> None:
        """點擊關閉結帳 Modal 按鈕。對應 TestComplete close-modal。"""
        self._js_click("close-modal")
