"""
Shopping Cart Page Object - 購物車頁面物件
"""
from selenium.webdriver.common.by import By
from page_objects.base_page import BasePage


class ShoppingCartPage(BasePage):
    """購物車頁面物件"""
    
    # 元素定位器
    CART_COUNT = (By.ID, "cart-count")
    CART_TOTAL = (By.ID, "cart-total")
    CLEAR_CART_BUTTON = (By.ID, "clear-cart")
    CHECKOUT_BUTTON = (By.ID, "checkout")
    CHECKOUT_MESSAGE = (By.ID, "checkout-message")
    
    # 購物車項目（動態元素）
    CART_ITEMS = (By.CLASS_NAME, "cart-item")
    
    def __init__(self, driver):
        """初始化購物車頁面"""
        super().__init__(driver)
    
    def get_cart_count(self):
        """
        取得購物車件數

        Returns:
            str: 購物車件數文字（例如："1 件"）
        """
        return self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"cart-count\"]');"
            "return el ? el.textContent.trim() : '';"
        )

    def get_cart_total(self):
        """
        取得購物車總計（使用 JS textContent，對隱藏元素也有效）

        Returns:
            str: 購物車總計文字（例如："NT$ 30"）
        """
        return self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"cart-total\"]');"
            "return el ? el.textContent.trim() : '';"
        )
    
    def click_clear_cart(self):
        """點擊清空購物車按鈕（data-testid=clear-cart，使用 JS）"""
        self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"clear-cart\"]');"
            "if (el) el.click();"
        )

    def click_checkout(self):
        """點擊結帳按鈕（data-testid=checkout，使用 JS）"""
        self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"checkout\"]');"
            "if (el) el.click();"
        )

    def get_checkout_message(self):
        """
        取得結帳訊息

        Returns:
            str: 結帳訊息文字
        """
        return self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"checkout-message\"]');"
            "return el ? el.textContent.trim() : '';"
        )
    
    def is_checkout_message_visible(self):
        """
        檢查結帳 Modal 是否開啟（以 CSS class 'is-open' 判斷）

        Returns:
            bool: Modal 是否可見
        """
        modal = self.driver.find_element(By.ID, "checkout-modal")
        return "is-open" in (modal.get_attribute("class") or "")
    
    def get_cart_items_count(self):
        """
        取得購物車內的商品項目數量
        
        Returns:
            int: 商品項目數量
        """
        items = self.find_elements(self.CART_ITEMS)
        return len(items)
    
    def get_product_quantity(self, product_name):
        """
        取得特定商品的數量

        Args:
            product_name: 商品名稱（蘋果、香蕉、牛奶）

        Returns:
            str: 商品數量
        """
        product_id = self._get_product_id(product_name)
        # data-testid="qty-value-{product_id}" (動態渲染)
        return self.driver.execute_script(
            f"var el = document.querySelector(\"[data-testid='qty-value-{product_id}']\");"
            "return el ? el.textContent.trim() : '';"
        )

    def click_increase_quantity(self, product_name):
        """
        點擊增加商品數量按鈕

        Args:
            product_name: 商品名稱
        """
        product_id = self._get_product_id(product_name)
        # data-testid="qty-plus-{product_id}" (動態渲染)
        self.driver.execute_script(
            f"var el = document.querySelector(\"[data-testid='qty-plus-{product_id}']\");"
            "if (el) el.click();"
        )

    def click_decrease_quantity(self, product_name):
        """
        點擊減少商品數量按鈕

        Args:
            product_name: 商品名稱
        """
        product_id = self._get_product_id(product_name)
        # data-testid="qty-minus-{product_id}" (動態渲染)
        self.driver.execute_script(
            f"var el = document.querySelector(\"[data-testid='qty-minus-{product_id}']\");"
            "if (el) el.click();"
        )

    def click_remove_product(self, product_name):
        """
        點擊移除商品按鈕

        Args:
            product_name: 商品名稱
        """
        product_id = self._get_product_id(product_name)
        # data-testid="remove-{product_id}" (動態渲染)
        self.driver.execute_script(
            f"var el = document.querySelector(\"[data-testid='remove-{product_id}']\");"
            "if (el) el.click();"
        )
    
    def is_cart_empty(self):
        """
        檢查購物車是否為空
        
        Returns:
            bool: 購物車是否為空
        """
        count_text = self.get_cart_count()
        total_text = self.get_cart_total()
        return count_text == "0 件" and total_text == "NT$ 0"
    
    def _get_product_id(self, product_name):
        """
        根據商品名稱取得商品 ID
        
        Args:
            product_name: 商品名稱（蘋果、香蕉、牛奶）
            
        Returns:
            str: 商品 ID（apple、banana、milk）
        """
        product_ids = {
            "蘋果": "apple",
            "香蕉": "banana",
            "牛奶": "milk"
        }
        return product_ids.get(product_name, product_name.lower())
    
    def clear_cart(self):
        """清空購物車（使用清空按鈕；若購物車為空則直接返回）"""
        count_text = self.driver.execute_script(
            "var el = document.querySelector('[data-testid=\"cart-count\"]');"
            "return el ? el.textContent.trim() : '0 件';"
        )
        if count_text and count_text != "0 件":
            self.driver.execute_script(
                "var el = document.querySelector('[data-testid=\"clear-cart\"]');"
                "if (el) el.click();"
            )
            self.wait_for_element(self.CART_COUNT)
