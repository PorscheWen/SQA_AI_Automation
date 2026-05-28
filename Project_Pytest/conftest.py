"""
Pytest 全域 fixtures 與 hooks。

CLI 選項：
  --browser   chrome | firefox  (預設 chrome)
  --headless  無頭模式
  --base-url  應用程式根 URL (預設 http://localhost:8888)
"""
import os
import pytest
from datetime import datetime
from selenium import webdriver
from selenium.webdriver.chrome.options import Options as ChromeOptions
from selenium.webdriver.firefox.options import Options as FirefoxOptions


# ---------------------------------------------------------------------------
# CLI options
# ---------------------------------------------------------------------------

def pytest_addoption(parser):
    parser.addoption(
        "--browser", action="store", default="chrome",
        help="Browser to run tests: chrome or firefox",
    )
    parser.addoption(
        "--headless", action="store_true", default=False,
        help="Run browser in headless mode",
    )
    parser.addoption(
        "--base-url", action="store", default="http://localhost:8888",
        help="Base URL of the application under test",
    )


# ---------------------------------------------------------------------------
# Browser factory helpers
# ---------------------------------------------------------------------------

def _chrome_options(headless: bool) -> ChromeOptions:
    opts = ChromeOptions()
    if headless:
        opts.add_argument("--headless=new")
        opts.add_argument("--disable-gpu")
    opts.add_argument("--no-sandbox")
    opts.add_argument("--disable-dev-shm-usage")
    opts.add_argument("--disable-extensions")
    opts.add_argument("--window-size=1280,800")
    opts.add_experimental_option("excludeSwitches", ["enable-logging"])

    # Locate Chrome/Chromium binary (Linux CI / local)
    for candidate in (
        "/usr/bin/chromium-browser",
        "/usr/bin/chromium",
        "/usr/bin/google-chrome",
        "/usr/bin/google-chrome-stable",
    ):
        if os.path.exists(candidate):
            opts.binary_location = candidate
            break

    return opts


def _build_chrome(headless: bool) -> webdriver.Chrome:
    opts = _chrome_options(headless)
    try:
        from webdriver_manager.chrome import ChromeDriverManager
        from selenium.webdriver.chrome.service import Service
        return webdriver.Chrome(
            service=Service(ChromeDriverManager().install()), options=opts
        )
    except Exception:
        return webdriver.Chrome(options=opts)


def _build_firefox(headless: bool) -> webdriver.Firefox:
    opts = FirefoxOptions()
    if headless:
        opts.add_argument("--headless")
    opts.add_argument("--width=1280")
    opts.add_argument("--height=800")
    try:
        from webdriver_manager.firefox import GeckoDriverManager
        from selenium.webdriver.firefox.service import Service
        return webdriver.Firefox(
            service=Service(GeckoDriverManager().install()), options=opts
        )
    except Exception:
        return webdriver.Firefox(options=opts)


# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------

@pytest.fixture(scope="session")
def base_url(request) -> str:
    """應用程式根 URL。"""
    return request.config.getoption("--base-url")


@pytest.fixture(scope="function")
def browser(request):
    """
    每個測試函數建立獨立的瀏覽器實例。
    支援 --browser、--headless CLI 選項。
    """
    browser_name = request.config.getoption("--browser").lower()
    headless = request.config.getoption("--headless")

    if browser_name == "chrome":
        driver = _build_chrome(headless)
    elif browser_name == "firefox":
        driver = _build_firefox(headless)
    else:
        raise ValueError(f"不支援的瀏覽器: {browser_name}")

    driver.implicitly_wait(10)
    yield driver
    driver.quit()


@pytest.fixture(scope="session", autouse=True)
def _ensure_reports_dir():
    """確保 reports/ 和 reports/screenshots/ 目錄存在。"""
    base = os.path.join(os.path.dirname(__file__), "reports")
    os.makedirs(os.path.join(base, "screenshots"), exist_ok=True)


# ---------------------------------------------------------------------------
# Hooks
# ---------------------------------------------------------------------------

@pytest.hookimpl(tryfirst=True, hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """讓測試結果可在 fixture 內存取。"""
    outcome = yield
    rep = outcome.get_result()
    setattr(item, f"rep_{rep.when}", rep)
