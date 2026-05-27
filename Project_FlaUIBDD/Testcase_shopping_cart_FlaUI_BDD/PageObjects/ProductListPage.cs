using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace ShoppingCartTests.PageObjects
{
    /// <summary>
    /// 產品列表頁面物件
    /// </summary>
    public class ProductListPage : BasePage
    {
        // 元素定位器 (AutomationId)
        private const string AddAppleButtonId = "add-apple";
        private const string AddBananaButtonId = "add-banana";
        private const string AddMilkButtonId = "add-milk";

        public ProductListPage(Window window, UIA3Automation automation)
            : base(window, automation)
        {
        }

        /// <summary>
        /// 點擊加入蘋果按鈕
        /// </summary>
        public void ClickAddApple()
        {
            ClickElement(AddAppleButtonId);
        }

        /// <summary>
        /// 點擊加入香蕉按鈕
        /// </summary>
        public void ClickAddBanana()
        {
            ClickElement(AddBananaButtonId);
        }

        /// <summary>
        /// 點擊加入牛奶按鈕
        /// </summary>
        public void ClickAddMilk()
        {
            ClickElement(AddMilkButtonId);
        }

        /// <summary>
        /// 根據商品名稱加入商品
        /// </summary>
        public void AddItem(string itemName)
        {
            var automationId = $"add-{itemName.ToLower()}";
            ClickElement(automationId);
        }

        /// <summary>
        /// 批次加入多個商品
        /// </summary>
        public void AddMultipleItems(params string[] items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }
    }
}
