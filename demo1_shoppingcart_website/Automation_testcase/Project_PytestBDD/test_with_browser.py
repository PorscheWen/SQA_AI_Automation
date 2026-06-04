#!/usr/bin/env python3
"""
使用真實瀏覽器的 E2E 測試
展示如何使用 Selenium WebDriver 對購物車網頁進行實際測試
"""
import sys
import os
import time

# 添加專案路徑
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import pytest
from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
from page_objects.product_list_page import ProductListPage
from page_objects.shopping_cart_page import ShoppingCartPage


# 測試網址
BASE_URL = "http://localhost:8888/"


@pytest.fixture
def browser():
    """建立瀏覽器實例"""
    print("\n🌐 啟動 Chrome 瀏覽器...")
    
    # 設定 Chrome 選項
    chrome_options = Options()
    chrome_options.add_argument("--headless=new")  # 無頭模式
    chrome_options.add_argument("--no-sandbox")
    chrome_options.add_argument("--disable-dev-shm-usage")
    chrome_options.add_argument("--window-size=1920,1080")
    
    # 建立 WebDriver
    service = Service(ChromeDriverManager().install())
    driver = webdriver.Chrome(service=service, options=chrome_options)
    driver.implicitly_wait(10)
    
    yield driver
    
    print("  🔒 關閉瀏覽器...")
    driver.quit()


def test_add_single_product(browser):
    """測試加入單一商品到購物車"""
    print("\n🧪 測試 TC01: 加入單一商品到購物車...")
    
    # 開啟購物車頁面
    browser.get(BASE_URL)
    time.sleep(1)
    
    # 建立 Page Objects
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 檢查初始狀態
    initial_count = cart_page.get_cart_count()
    print(f"  📊 初始購物車件數: {initial_count}")
    assert initial_count == "0 件", f"初始件數應為 0 件，實際為 {initial_count}"
    
    # 加入蘋果
    product_page.add_apple()
    time.sleep(0.5)
    
    # 驗證購物車
    cart_count = cart_page.get_cart_count()
    cart_total = cart_page.get_cart_total()
    print(f"  ✅ 購物車件數: {cart_count}")
    print(f"  ✅ 購物車總計: {cart_total}")
    
    assert cart_count == "1 件", f"件數應為 1 件，實際為 {cart_count}"
    assert "30" in cart_total, f"總計應包含 30，實際為 {cart_total}"
    
    print("  🎉 測試通過！\n")


def test_add_multiple_products(browser):
    """測試加入多種商品到購物車"""
    print("🧪 測試 TC02: 加入多種商品到購物車...")
    
    browser.get(BASE_URL)
    time.sleep(1)
    
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 加入多種商品
    product_page.add_apple()
    time.sleep(0.3)
    product_page.add_banana()
    time.sleep(0.3)
    product_page.add_milk()
    time.sleep(0.5)
    
    # 驗證購物車
    cart_count = cart_page.get_cart_count()
    cart_total = cart_page.get_cart_total()
    print(f"  ✅ 購物車件數: {cart_count}")
    print(f"  ✅ 購物車總計: {cart_total}")
    
    assert cart_count == "3 件", f"件數應為 3 件，實際為 {cart_count}"
    # 蘋果 30 + 香蕉 25 + 牛奶 40 = 95
    assert "95" in cart_total, f"總計應包含 95，實際為 {cart_total}"
    
    print("  🎉 測試通過！\n")


def test_increase_quantity(browser):
    """測試增加購物車商品數量"""
    print("🧪 測試 TC03: 增加購物車商品數量...")
    
    browser.get(BASE_URL)
    time.sleep(1)
    
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 加入蘋果
    product_page.add_apple()
    time.sleep(0.5)
    
    # 增加數量
    cart_page.click_increase_quantity("蘋果")
    time.sleep(0.5)
    
    # 驗證購物車
    cart_count = cart_page.get_cart_count()
    cart_total = cart_page.get_cart_total()
    print(f"  ✅ 購物車件數: {cart_count}")
    print(f"  ✅ 購物車總計: {cart_total}")
    
    assert cart_count == "2 件", f"件數應為 2 件，實際為 {cart_count}"
    assert "60" in cart_total, f"總計應包含 60，實際為 {cart_total}"
    
    print("  🎉 測試通過！\n")


def test_remove_product(browser):
    """測試從購物車移除商品"""
    print("🧪 測試 TC04: 從購物車移除商品...")
    
    browser.get(BASE_URL)
    time.sleep(1)
    
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 加入多種商品
    product_page.add_apple()
    time.sleep(0.3)
    product_page.add_banana()
    time.sleep(0.5)
    
    # 移除香蕉
    cart_page.click_remove_product("香蕉")
    time.sleep(0.5)
    
    # 驗證購物車
    cart_count = cart_page.get_cart_count()
    cart_total = cart_page.get_cart_total()
    print(f"  ✅ 購物車件數: {cart_count}")
    print(f"  ✅ 購物車總計: {cart_total}")
    
    assert cart_count == "1 件", f"件數應為 1 件，實際為 {cart_count}"
    assert "30" in cart_total, f"總計應包含 30，實際為 {cart_total}"
    
    print("  🎉 測試通過！\n")


def test_clear_cart(browser):
    """測試清空購物車"""
    print("🧪 測試 TC05: 清空購物車...")
    
    browser.get(BASE_URL)
    time.sleep(1)
    
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 加入商品
    product_page.add_apple()
    time.sleep(0.3)
    product_page.add_milk()
    time.sleep(0.5)
    
    # 清空購物車
    cart_page.click_clear_cart()
    time.sleep(0.5)
    
    # 驗證購物車為空
    is_empty = cart_page.is_cart_empty()
    cart_count = cart_page.get_cart_count()
    cart_total = cart_page.get_cart_total()
    print(f"  ✅ 購物車是否為空: {is_empty}")
    print(f"  ✅ 購物車件數: {cart_count}")
    print(f"  ✅ 購物車總計: {cart_total}")
    
    assert is_empty == True, "購物車應該是空的"
    assert cart_count == "0 件", f"件數應為 0 件，實際為 {cart_count}"
    
    print("  🎉 測試通過！\n")


def test_checkout(browser):
    """測試結帳流程"""
    print("🧪 測試 TC06: 測試結帳流程...")
    
    browser.get(BASE_URL)
    time.sleep(1)
    
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 加入商品
    product_page.add_banana()
    time.sleep(0.5)
    
    # 結帳
    cart_page.click_checkout()
    time.sleep(0.5)
    
    # 驗證 alert
    try:
        alert = browser.switch_to.alert
        alert_text = alert.text
        print(f"  ✅ Alert 訊息: {alert_text}")
        assert "結帳成功" in alert_text, f"Alert 應包含'結帳成功'，實際為 {alert_text}"
        alert.accept()
        time.sleep(0.5)
    except Exception as e:
        print(f"  ❌ 無法取得 alert: {e}")
        raise
    
    # 驗證購物車已清空
    is_empty = cart_page.is_cart_empty()
    print(f"  ✅ 購物車是否為空: {is_empty}")
    assert is_empty == True, "結帳後購物車應該是空的"
    
    print("  🎉 測試通過！\n")


@pytest.mark.parametrize("product_name,expected_price", [
    ("蘋果", "30"),
    ("香蕉", "25"),
    ("牛奶", "40")
])
def test_add_different_products(browser, product_name, expected_price):
    """測試加入不同商品的功能（參數化）"""
    print(f"🧪 測試 TC07: 驗證 {product_name} 的加入功能...")
    
    browser.get(BASE_URL)
    time.sleep(1)
    
    product_page = ProductListPage(browser)
    cart_page = ShoppingCartPage(browser)
    
    # 加入商品
    product_page.add_product(product_name)
    time.sleep(0.5)
    
    # 驗證購物車
    cart_count = cart_page.get_cart_count()
    cart_total = cart_page.get_cart_total()
    print(f"  ✅ 購物車件數: {cart_count}")
    print(f"  ✅ 購物車總計: {cart_total}")
    
    assert cart_count == "1 件", f"件數應為 1 件，實際為 {cart_count}"
    assert expected_price in cart_total, f"總計應包含 {expected_price}，實際為 {cart_total}"
    
    print("  🎉 測試通過！\n")


def main():
    """主測試函數 - 使用 pytest 執行"""
    print("=" * 70)
    print("🧪 Pytest BDD 專案 - E2E 測試（使用真實瀏覽器）")
    print("=" * 70)
    print()
    print("說明: 此腳本使用 Selenium WebDriver 對購物車進行實際測試")
    print(f"測試網址: {BASE_URL}")
    print()
    print("⚠️  注意: 請確保測試網頁已經啟動")
    print("   啟動命令: cd ../demo/shopping_cart && python serve.py")
    print()
    print("執行測試...")
    print()
    
    # 使用 pytest 執行測試
    pytest_args = [
        __file__,
        "-v",
        "--tb=short",
        "-s"  # 顯示 print 輸出
    ]
    
    exit_code = pytest.main(pytest_args)
    
    print()
    print("=" * 70)
    if exit_code == 0:
        print("✅ 所有 E2E 測試通過！")
    else:
        print(f"❌ 測試失敗 (退出碼: {exit_code})")
    print("=" * 70)
    print()
    
    return exit_code


if __name__ == "__main__":
    sys.exit(main())
