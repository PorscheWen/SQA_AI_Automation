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
        return self.get_element_text(self.CART_COUNT)
    
    def get_cart_total(self):
        """
        取得購物車總計
        
        Returns:
            str: 購物車總計文字（例如："NT$ 30"）
        """
        return self.get_element_text(self.CART_TOTAL)
    
    def click_clear_cart(self):
        """點擊清空購物車按鈕"""
        self.click_element(self.CLEAR_CART_BUTTON)
    
    def click_checkout(self):
        """點擊結帳按鈕"""
        self.click_element(self.CHECKOUT_BUTTON)
    
    def get_checkout_message(self):
        """
        取得結帳訊息
        
        Returns:
            str: 結帳訊息文字
        """
        return self.get_element_text(self.CHECKOUT_MESSAGE)
    
    def is_checkout_message_visible(self):
        """
        檢查結帳訊息是否顯示
        
        Returns:
            bool: 訊息是否可見
        """
        return self.is_element_visible(self.CHECKOUT_MESSAGE, timeout=5)
    
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
        quantity_id = f"quantity-{self._get_product_id(product_name)}"
        locator = (By.ID, quantity_id)
        return self.get_element_text(locator)
    
    def click_increase_quantity(self, product_name):
        """
        點擊增加商品數量按鈕
        
        Args:
            product_name: 商品名稱
        """
        increase_id = f"increase-{self._get_product_id(product_name)}"
        locator = (By.ID, increase_id)
        self.click_element(locator)
    
    def click_decrease_quantity(self, product_name):
        """
        點擊減少商品數量按鈕
        
        Args:
            product_name: 商品名稱
        """
        decrease_id = f"decrease-{self._get_product_id(product_name)}"
        locator = (By.ID, decrease_id)
        self.click_element(locator)
    
    def click_remove_product(self, product_name):
        """
        點擊移除商品按鈕
        
        Args:
            product_name: 商品名稱
        """
        remove_id = f"remove-{self._get_product_id(product_name)}"
        locator = (By.ID, remove_id)
        self.click_element(locator)
    
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
        """清空購物車（直接執行 JavaScript）"""
        self.execute_script("localStorage.clear(); location.reload();")
        self.wait_for_element(self.CART_COUNT)
