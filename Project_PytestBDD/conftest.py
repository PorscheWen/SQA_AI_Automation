"""
Pytest Configuration and Fixtures
Pytest 配置和 Fixtures
"""
import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options as ChromeOptions
from selenium.webdriver.chrome.service import Service as ChromeService
from selenium.webdriver.firefox.options import Options as FirefoxOptions
from selenium.webdriver.firefox.service import Service as FirefoxService
from webdriver_manager.chrome import ChromeDriverManager
from webdriver_manager.firefox import GeckoDriverManager
import os
from datetime import datetime


def pytest_addoption(parser):
    """新增 pytest 命令列選項"""
    parser.addoption(
        "--browser",
        action="store",
        default="chrome",
        help="Browser to run tests: chrome or firefox"
    )
    parser.addoption(
        "--headless",
        action="store_true",
        default=False,
        help="Run browser in headless mode"
    )
    parser.addoption(
        "--base-url",
        action="store",
        default="http://localhost:8888",
        help="Base URL for the application"
    )


@pytest.fixture(scope='function')
def browser(request):
    """
    瀏覽器 fixture
    每個測試函數會創建新的瀏覽器實例
    """
    browser_name = request.config.getoption("--browser")
    headless = request.config.getoption("--headless")
    
    driver = None
    
    if browser_name.lower() == "chrome":
        chrome_options = ChromeOptions()
        if headless:
            chrome_options.add_argument("--headless=new")
            chrome_options.add_argument("--disable-gpu")
        chrome_options.add_argument("--no-sandbox")
        chrome_options.add_argument("--disable-dev-shm-usage")
        chrome_options.add_argument("--disable-extensions")
        chrome_options.add_argument("--disable-logging")
        chrome_options.add_argument("--disable-in-process-stack-traces")
        chrome_options.add_argument("--log-level=3")
        chrome_options.add_argument("--remote-debugging-port=9222")
        chrome_options.add_argument("--window-size=1920,1080")
        chrome_options.add_argument("--user-data-dir=/tmp/chrome-data")
        chrome_options.add_experimental_option('excludeSwitches', ['enable-logging'])
        chrome_options.binary_location = "/usr/bin/chromium-browser"
        
        service = ChromeService(ChromeDriverManager().install())
        driver = webdriver.Chrome(service=service, options=chrome_options)
        
    elif browser_name.lower() == "firefox":
        firefox_options = FirefoxOptions()
        if headless:
            firefox_options.add_argument("--headless")
        firefox_options.add_argument("--width=1920")
        firefox_options.add_argument("--height=1080")
        
        service = FirefoxService(GeckoDriverManager().install())
        driver = webdriver.Firefox(service=service, options=firefox_options)
    
    else:
        raise ValueError(f"不支援的瀏覽器: {browser_name}")
    
    # 設定隱式等待
    driver.implicitly_wait(10)
    
    yield driver
    
    # 測試結束後關閉瀏覽器
    driver.quit()


@pytest.fixture(scope='session')
def base_url(request):
    """取得基礎 URL"""
    return request.config.getoption("--base-url")


@pytest.fixture(scope='function')
def screenshot_on_failure(request, browser):
    """測試失敗時自動截圖"""
    yield
    
    if request.node.rep_call.failed:
        # 測試失敗，擷取截圖
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        test_name = request.node.name
        filename = f"FAILED_{test_name}_{timestamp}.png"
        
        screenshots_dir = os.path.join(
            os.path.dirname(__file__), 
            'reports', 
            'screenshots'
        )
        os.makedirs(screenshots_dir, exist_ok=True)
        
        filepath = os.path.join(screenshots_dir, filename)
        browser.save_screenshot(filepath)
        print(f"\n測試失敗截圖: {filepath}")


@pytest.hookimpl(tryfirst=True, hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """
    Hook to make test result available in fixtures
    讓測試結果在 fixture 中可用
    """
    outcome = yield
    rep = outcome.get_result()
    setattr(item, f"rep_{rep.when}", rep)


@pytest.fixture(scope='session', autouse=True)
def test_report_setup():
    """設定測試報告目錄"""
    reports_dir = os.path.join(os.path.dirname(__file__), 'reports')
    screenshots_dir = os.path.join(reports_dir, 'screenshots')
    os.makedirs(screenshots_dir, exist_ok=True)
    print(f"\n測試報告目錄: {reports_dir}")
    print(f"截圖目錄: {screenshots_dir}")


# BDD Scenario hooks
def pytest_bdd_before_scenario(request, feature, scenario):
    """在每個場景執行前"""
    print(f"\n▶️  開始場景: {scenario.name}")


def pytest_bdd_after_scenario(request, feature, scenario):
    """在每個場景執行後"""
    print(f"✅ 完成場景: {scenario.name}")


def pytest_bdd_step_error(request, feature, scenario, step, step_func, step_func_args, exception):
    """步驟執行錯誤時"""
    print(f"\n❌ 步驟失敗: {step.name}")
    print(f"   錯誤訊息: {exception}")
