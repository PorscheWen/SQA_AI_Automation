"""
Product List Page Object - 商品列表頁面物件
"""
from selenium.webdriver.common.by import By
from page_objects.base_page import BasePage


class ProductListPage(BasePage):
    """商品列表頁面物件"""
    
    # 元素定位器
    ADD_APPLE_BUTTON = (By.ID, "add-apple")
    ADD_BANANA_BUTTON = (By.ID, "add-banana")
    ADD_MILK_BUTTON = (By.ID, "add-milk")
    
    def __init__(self, driver):
        """初始化商品列表頁面"""
        super().__init__(driver)
    
    def add_apple(self):
        """點擊加入蘋果按鈕"""
        self.click_element(self.ADD_APPLE_BUTTON)
    
    def add_banana(self):
        """點擊加入香蕉按鈕"""
        self.click_element(self.ADD_BANANA_BUTTON)
    
    def add_milk(self):
        """點擊加入牛奶按鈕"""
        self.click_element(self.ADD_MILK_BUTTON)
    
    def add_product(self, product_name):
        """
        根據商品名稱加入商品
        
        Args:
            product_name: 商品名稱（蘋果、香蕉、牛奶）
        """
        product_buttons = {
            "蘋果": self.ADD_APPLE_BUTTON,
            "香蕉": self.ADD_BANANA_BUTTON,
            "牛奶": self.ADD_MILK_BUTTON
        }
        
        if product_name in product_buttons:
            self.click_element(product_buttons[product_name])
        else:
            raise ValueError(f"未知的商品名稱: {product_name}")
    
    def is_product_button_visible(self, product_name):
        """
        檢查商品按鈕是否可見
        
        Args:
            product_name: 商品名稱
            
        Returns:
            bool: 按鈕是否可見
        """
        product_buttons = {
            "蘋果": self.ADD_APPLE_BUTTON,
            "香蕉": self.ADD_BANANA_BUTTON,
            "牛奶": self.ADD_MILK_BUTTON
        }
        
        if product_name in product_buttons:
            return self.is_element_visible(product_buttons[product_name])
        return False
