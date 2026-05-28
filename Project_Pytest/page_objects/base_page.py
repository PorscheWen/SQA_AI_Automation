"""Base page object：提供通用的 Selenium 操作封裝。"""
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException


class BasePage:
    """所有頁面物件的基礎類別。"""

    def __init__(self, driver, timeout: int = 10):
        self.driver = driver
        self.timeout = timeout
        self.wait = WebDriverWait(driver, timeout)

    # ------------------------------------------------------------------
    # Element finders / interactions
    # ------------------------------------------------------------------

    def find_element(self, locator):
        """等待元素出現於 DOM 並回傳。"""
        try:
            return self.wait.until(EC.presence_of_element_located(locator))
        except TimeoutException:
            raise TimeoutException(f"找不到元素: {locator}")

    def click_element(self, locator):
        """等待元素可點擊後點擊。"""
        element = self.wait.until(EC.element_to_be_clickable(locator))
        element.click()

    def get_element_text(self, locator) -> str:
        """取得元素可見文字（strip 空白）。"""
        return self.find_element(locator).text.strip()

    def is_element_visible(self, locator, timeout: int = None) -> bool:
        """檢查元素是否可見。"""
        try:
            t = timeout if timeout is not None else self.timeout
            WebDriverWait(self.driver, t).until(
                EC.visibility_of_element_located(locator)
            )
            return True
        except TimeoutException:
            return False

    # ------------------------------------------------------------------
    # Page-level
    # ------------------------------------------------------------------

    def get_page_title(self) -> str:
        """取得頁面 <title>。"""
        return self.driver.title
