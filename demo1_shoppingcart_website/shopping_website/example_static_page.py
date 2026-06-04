# -*- coding: utf-8 -*-
"""
TestComplete Python 範例：靜態網頁驗證

用途：驗證 https://example.com 頁面標題與 H1 文字。
使用方式：複製到 TestComplete Script 單元，執行 TestMain()。
"""

import Sys
from TestComplete import *


def open_browser(url, timeout_ms=30000):
    """開啟 Chrome 並等待頁面載入完成。"""
    Browsers.Item[btChrome].Run(url)
    Sys.Browser("*").WaitPage("*", timeout_ms)


def verify_h1_text(expected_text):
    """以 XPath 找到 H1 並驗證文字內容。"""
    page = Sys.Browser("*").Page("*")
    heading = page.FindChildByXPath("//h1", 10)

    if heading is None:
        Log.Error("H1 element not found")
        return False

    actual = heading.contentText
    Log.Message("H1 text: " + actual)
    aqObject.CheckProperty(heading, "contentText", cmpEqual, expected_text)
    return True


def TestMain():
    """TestComplete 入口函式。"""
    url = "https://example.com"

    try:
        open_browser(url)
        verify_h1_text("Example Domain")
        Log.Message("Test passed: static page verification OK")
    except Exception as e:
        Log.Error("Test failed: " + str(e))
        try:
            Sys.Browser("*").Page("*").Picture().SaveToFile("error_screenshot.png")
            Log.Message("Screenshot saved: error_screenshot.png")
        except Exception:
            pass
        raise
    finally:
        try:
            Sys.Browser("*").Close()
        except Exception:
            pass
