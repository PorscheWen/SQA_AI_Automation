"""
Base Page Object - 所有頁面物件的基礎類別
提供通用的 Selenium 操作方法
"""
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException, NoSuchElementException
import os
from datetime import datetime


class BasePage:
    """基礎頁面物件類別"""
    
    def __init__(self, driver, timeout=10):
        """
        初始化基礎頁面
        
        Args:
            driver: Selenium WebDriver 實例
            timeout: 元素等待超時時間（秒）
        """
        self.driver = driver
        self.timeout = timeout
        self.wait = WebDriverWait(driver, timeout)
    
    def find_element(self, locator):
        """
        尋找單一元素
        
        Args:
            locator: 元素定位器 (By.ID, "element_id")
            
        Returns:
            WebElement: 找到的元素
        """
        try:
            return self.wait.until(EC.presence_of_element_located(locator))
        except TimeoutException:
            raise TimeoutException(f"無法找到元素: {locator}")
    
    def find_elements(self, locator):
        """
        尋找多個元素
        
        Args:
            locator: 元素定位器
            
        Returns:
            list: 元素列表
        """
        try:
            return self.wait.until(EC.presence_of_all_elements_located(locator))
        except TimeoutException:
            return []
    
    def click_element(self, locator):
        """
        點擊元素
        
        Args:
            locator: 元素定位器
        """
        element = self.wait.until(EC.element_to_be_clickable(locator))
        element.click()
    
    def get_element_text(self, locator):
        """
        取得元素文字
        
        Args:
            locator: 元素定位器
            
        Returns:
            str: 元素文字內容
        """
        element = self.find_element(locator)
        return element.text.strip()
    
    def is_element_visible(self, locator, timeout=None):
        """
        檢查元素是否可見
        
        Args:
            locator: 元素定位器
            timeout: 自訂超時時間
            
        Returns:
            bool: 元素是否可見
        """
        try:
            wait_time = timeout if timeout else self.timeout
            WebDriverWait(self.driver, wait_time).until(
                EC.visibility_of_element_located(locator)
            )
            return True
        except TimeoutException:
            return False
    
    def is_element_present(self, locator):
        """
        檢查元素是否存在於 DOM
        
        Args:
            locator: 元素定位器
            
        Returns:
            bool: 元素是否存在
        """
        try:
            self.driver.find_element(*locator)
            return True
        except NoSuchElementException:
            return False
    
    def wait_for_element(self, locator, timeout=None):
        """
        等待元素出現
        
        Args:
            locator: 元素定位器
            timeout: 自訂超時時間
            
        Returns:
            WebElement: 找到的元素
        """
        wait_time = timeout if timeout else self.timeout
        return WebDriverWait(self.driver, wait_time).until(
            EC.presence_of_element_located(locator)
        )
    
    def wait_for_element_clickable(self, locator, timeout=None):
        """
        等待元素可點擊
        
        Args:
            locator: 元素定位器
            timeout: 自訂超時時間
            
        Returns:
            WebElement: 可點擊的元素
        """
        wait_time = timeout if timeout else self.timeout
        return WebDriverWait(self.driver, wait_time).until(
            EC.element_to_be_clickable(locator)
        )
    
    def take_screenshot(self, name):
        """
        擷取螢幕截圖
        
        Args:
            name: 截圖檔案名稱
            
        Returns:
            str: 截圖檔案路徑
        """
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        filename = f"{name}_{timestamp}.png"
        screenshots_dir = os.path.join(os.path.dirname(__file__), '..', 'reports', 'screenshots')
        os.makedirs(screenshots_dir, exist_ok=True)
        filepath = os.path.join(screenshots_dir, filename)
        self.driver.save_screenshot(filepath)
        print(f"截圖已儲存: {filepath}")
        return filepath
    
    def get_page_title(self):
        """
        取得頁面標題
        
        Returns:
            str: 頁面標題
        """
        return self.driver.title
    
    def get_current_url(self):
        """
        取得當前 URL
        
        Returns:
            str: 當前 URL
        """
        return self.driver.current_url
    
    def refresh_page(self):
        """重新整理頁面"""
        self.driver.refresh()
    
    def execute_script(self, script, *args):
        """
        執行 JavaScript
        
        Args:
            script: JavaScript 程式碼
            *args: 傳遞給 JavaScript 的參數
            
        Returns:
            執行結果
        """
        return self.driver.execute_script(script, *args)
