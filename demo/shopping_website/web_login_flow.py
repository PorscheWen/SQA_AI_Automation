# -*- coding: utf-8 -*-
"""
TestComplete Python 範例：登入流程

用途：示範 Name Mapping + 函式拆分的登入測試結構。
使用前請依實際網站調整 BASE_URL、Aliases 名稱與測試資料。
"""

import Sys
from TestComplete import *

# --- 可調整設定 ---
BASE_URL = "https://example.com/login"
TEST_USERNAME = "testuser"
TEST_PASSWORD = "password123"
EXPECTED_TITLE = "Dashboard - Example"


def open_browser(url, timeout_ms=30000):
    """開啟 Chrome 並等待頁面載入完成。"""
    Browsers.Item[btChrome].Run(url)
    Sys.Browser("*").WaitPage("*", timeout_ms)


def login(username, password):
    """
    登入流程（需先在 Name Mapping 建立對應 Aliases）。

    預期 Mapping 結構：
        Aliases.browser.pageLogin
            ├── textboxUsername
            ├── textboxPassword
            └── buttonSubmit
    """
    page = Aliases.browser.pageLogin
    page.textboxUsername.SetText(username)
    page.textboxPassword.SetText(password)
    page.buttonSubmit.Click()
    page.Wait()


def verify_homepage_title(expected_title):
    """驗證登入後頁面標題。"""
    page = Aliases.browser.pageHome
    actual = page.title
    Log.Message("Page title: " + actual)

    if actual != expected_title:
        Log.Error("Title mismatch: expected=%s, actual=%s" % (expected_title, actual))
        return False

    Log.Message("Title verification passed")
    return True


def take_screenshot(filename):
    """失敗或除錯時截圖。"""
    Sys.Browser("*").Page("*").Picture().SaveToFile(filename)
    Log.Message("Screenshot saved: " + filename)


def TestMain():
    """TestComplete 入口函式。"""
    try:
        open_browser(BASE_URL)
        login(TEST_USERNAME, TEST_PASSWORD)
        verify_homepage_title(EXPECTED_TITLE)
        Log.Message("Test passed: login flow OK")
    except Exception as e:
        Log.Error("Test failed: " + str(e))
        take_screenshot("login_error_screenshot.png")
        raise
    finally:
        try:
            Aliases.browser.Close()
        except Exception:
            pass
