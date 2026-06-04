#!/usr/bin/env python3
"""
驗證 Pytest BDD 專案結構腳本
在沒有瀏覽器的環境中驗證專案配置正確性
"""
import os
import sys


def print_status(check_name, status, message=""):
    """打印檢查狀態"""
    status_symbol = "✅" if status else "❌"
    print(f"{status_symbol} {check_name}", end="")
    if message:
        print(f": {message}")
    else:
        print()


def check_directory_structure():
    """檢查目錄結構"""
    print("\n📁 檢查目錄結構...")
    
    required_dirs = [
        "features",
        "step_definitions",
        "page_objects",
        "helpers",
        "reports"
    ]
    
    all_exist = True
    for dir_name in required_dirs:
        exists = os.path.isdir(dir_name)
        print_status(f"  {dir_name}/", exists)
        all_exist = all_exist and exists
    
    return all_exist


def check_required_files():
    """檢查必要檔案"""
    print("\n📄 檢查必要檔案...")
    
    required_files = [
        "features/shopping_cart.feature",
        "step_definitions/test_shopping_cart_steps.py",
        "page_objects/base_page.py",
        "page_objects/product_list_page.py",
        "page_objects/shopping_cart_page.py",
        "conftest.py",
        "pytest.ini",
        "requirements.txt",
        "README.md"
    ]
    
    all_exist = True
    for file_path in required_files:
        exists = os.path.isfile(file_path)
        print_status(f"  {file_path}", exists)
        all_exist = all_exist and exists
    
    return all_exist


def check_python_imports():
    """檢查 Python 套件導入"""
    print("\n📦 檢查 Python 套件...")
    
    packages = [
        ("pytest", "測試框架"),
        ("pytest_bdd", "BDD 框架"),
        ("selenium", "WebDriver"),
        ("webdriver_manager", "Driver 管理")
    ]
    
    all_imported = True
    for package, description in packages:
        try:
            __import__(package)
            print_status(f"  {package}", True, description)
        except ImportError:
            print_status(f"  {package}", False, f"{description} - 需安裝")
            all_imported = False
    
    return all_imported


def check_feature_file():
    """檢查 Feature 檔案內容"""
    print("\n📝 檢查 Feature 檔案...")
    
    feature_file = "features/shopping_cart.feature"
    if not os.path.isfile(feature_file):
        print_status("  Feature 檔案", False, "檔案不存在")
        return False
    
    with open(feature_file, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # 檢查場景數量
    scenario_count = content.count("場景:")
    scenario_outline_count = content.count("場景大綱:")
    total_scenarios = scenario_count + scenario_outline_count
    
    print_status(f"  場景數量", True, f"{scenario_count} 個場景")
    print_status(f"  場景大綱", True, f"{scenario_outline_count} 個參數化場景")
    print_status(f"  總測試場景", True, f"{total_scenarios} 個")
    
    # 檢查關鍵字
    keywords = ["功能:", "背景:", "假設", "當", "那麼", "而且"]
    for keyword in keywords:
        exists = keyword in content
        print_status(f"  包含 '{keyword}'", exists)
    
    return True


def check_step_definitions():
    """檢查步驟定義"""
    print("\n🔧 檢查步驟定義...")
    
    steps_file = "step_definitions/test_shopping_cart_steps.py"
    if not os.path.isfile(steps_file):
        print_status("  步驟定義檔案", False)
        return False
    
    with open(steps_file, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # 檢查裝飾器
    decorators = ["@given", "@when", "@then", "@scenarios"]
    for decorator in decorators:
        count = content.count(decorator)
        exists = count > 0
        print_status(f"  {decorator}", exists, f"使用 {count} 次")
    
    return True


def check_page_objects():
    """檢查 Page Objects"""
    print("\n🎯 檢查 Page Objects...")
    
    page_objects = [
        ("page_objects/base_page.py", "BasePage"),
        ("page_objects/product_list_page.py", "ProductListPage"),
        ("page_objects/shopping_cart_page.py", "ShoppingCartPage")
    ]
    
    all_exist = True
    for file_path, class_name in page_objects:
        if os.path.isfile(file_path):
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            has_class = f"class {class_name}" in content
            print_status(f"  {class_name}", has_class, f"in {file_path}")
            all_exist = all_exist and has_class
        else:
            print_status(f"  {file_path}", False)
            all_exist = False
    
    return all_exist


def main():
    """主函數"""
    print("="* 60)
    print("🧪 Pytest BDD 專案結構驗證")
    print("=" * 60)
    
    checks = [
        ("目錄結構", check_directory_structure),
        ("必要檔案", check_required_files),
        ("Python 套件", check_python_imports),
        ("Feature 檔案", check_feature_file),
        ("步驟定義", check_step_definitions),
        ("Page Objects", check_page_objects)
    ]
    
    results = []
    for name, check_func in checks:
        try:
            result = check_func()
            results.append((name, result))
        except Exception as e:
            print(f"❌ {name}: 檢查時發生錯誤 - {e}")
            results.append((name, False))
    
    # 總結
    print("\n" + "=" * 60)
    print("📊 驗證結果總結")
    print("=" * 60)
    
    passed = sum(1 for _, result in results if result)
    total = len(results)
    
    for name, result in results:
        status_symbol = "✅" if result else "❌"
        print(f"{status_symbol} {name}")
    
    print("\n" + "=" * 60)
    print(f"總計: {passed}/{total} 項檢查通過")
    
    if passed == total:
        print("✅ 專案結構完整且正確！")
        print("\n💡 下一步:")
        print("   1. 確保已安裝瀏覽器 (Chrome/Chromium/Firefox)")
        print("   2. 啟動測試網頁: python ../demo/shopping_cart/serve.py")
        print("   3. 執行測試: pytest --headless")
        return 0
    else:
        print(f"❌ 有 {total - passed} 項檢查未通過")
        print("\n建議:")
        print("   - 檢查是否安裝所有依賴: pip install -r requirements.txt")
        print("   - 確認所有檔案已正確創建")
        return 1


if __name__ == "__main__":
    sys.exit(main())
