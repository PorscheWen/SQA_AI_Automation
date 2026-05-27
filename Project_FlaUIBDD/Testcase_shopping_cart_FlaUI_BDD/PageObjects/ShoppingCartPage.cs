using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace ShoppingCartTests.PageObjects
{
    /// <summary>
    /// 購物車頁面物件
    /// </summary>
    public class ShoppingCartPage : BasePage
    {
        // 元素定位器 (AutomationId)
        private const string CartCountId = "cart-count";
        private const string CartTotalId = "cart-total";
        private const string CartEmptyMessageId = "cart-empty";
        private const string ClearCartButtonId = "clear-cart";
        private const string CheckoutButtonId = "checkout";
        private const string CheckoutMessageId = "checkout-message";
        private const string CheckoutTotalId = "checkout-total";
        private const string CloseModalButtonId = "close-modal";

        // 商品數量相關
        private const string QtyValuePrefix = "qty-value-";
        private const string QtyPlusPrefix = "qty-plus-";
        private const string QtyMinusPrefix = "qty-minus-";
        private const string RemoveItemPrefix = "remove-";

        public ShoppingCartPage(Window window, UIA3Automation automation)
            : base(window, automation)
        {
        }

        #region 購物車基本操作

        /// <summary>
        /// 取得購物車件數
        /// </summary>
        public string GetCartCount()
        {
            return GetElementText(CartCountId);
        }

        /// <summary>
        /// 取得購物車總計
        /// </summary>
        public string GetCartTotal()
        {
            return GetElementText(CartTotalId);
        }

        /// <summary>
        /// 取得空購物車訊息
        /// </summary>
        public string GetCartEmptyMessage()
        {
            return GetElementText(CartEmptyMessageId);
        }

        /// <summary>
        /// 點擊清空購物車按鈕
        /// </summary>
        public void ClickClearCart()
        {
            ClickElement(ClearCartButtonId);
        }

        /// <summary>
        /// 重置購物車（如果有商品則清空）
        /// </summary>
        public void ResetCart()
        {
            var count = GetCartCount();
            if (!string.IsNullOrEmpty(count) && count != "0 件")
            {
                ClickClearCart();
                WaitForWindow();
            }
        }

        #endregion

        #region 商品數量操作

        /// <summary>
        /// 取得指定商品的數量
        /// </summary>
        public string GetItemQuantity(string itemName)
        {
            var automationId = $"{QtyValuePrefix}{itemName.ToLower()}";
            return GetElementText(automationId);
        }

        /// <summary>
        /// 點擊增加商品數量按鈕
        /// </summary>
        public void ClickIncreaseQuantity(string itemName)
        {
            var automationId = $"{QtyPlusPrefix}{itemName.ToLower()}";
            ClickElement(automationId);
        }

        /// <summary>
        /// 點擊減少商品數量按鈕
        /// </summary>
        public void ClickDecreaseQuantity(string itemName)
        {
            var automationId = $"{QtyMinusPrefix}{itemName.ToLower()}";
            ClickElement(automationId);
        }

        /// <summary>
        /// 點擊移除商品按鈕
        /// </summary>
        public void ClickRemoveItem(string itemName)
        {
            var automationId = $"{RemoveItemPrefix}{itemName.ToLower()}";
            ClickElement(automationId);
        }

        #endregion

        #region 結帳操作

        /// <summary>
        /// 點擊結帳按鈕
        /// </summary>
        public void ClickCheckout()
        {
            ClickElement(CheckoutButtonId);
        }

        /// <summary>
        /// 取得結帳訊息
        /// </summary>
        public string GetCheckoutMessage()
        {
            return GetElementText(CheckoutMessageId);
        }

        /// <summary>
        /// 取得結帳總金額
        /// </summary>
        public string GetCheckoutTotal()
        {
            return GetElementText(CheckoutTotalId);
        }

        /// <summary>
        /// 關閉結帳視窗
        /// </summary>
        public void CloseCheckoutModal()
        {
            ClickElement(CloseModalButtonId);
        }

        #endregion

        #region 驗證方法

        /// <summary>
        /// 驗證購物車件數
        /// </summary>
        public bool VerifyCartCount(string expectedCount)
        {
            return VerifyElementText(CartCountId, expectedCount);
        }

        /// <summary>
        /// 驗證購物車總計
        /// </summary>
        public bool VerifyCartTotal(string expectedTotal)
        {
            return VerifyElementText(CartTotalId, expectedTotal);
        }

        /// <summary>
        /// 驗證商品數量
        /// </summary>
        public bool VerifyItemQuantity(string itemName, string expectedQuantity)
        {
            var automationId = $"{QtyValuePrefix}{itemName.ToLower()}";
            return VerifyElementText(automationId, expectedQuantity);
        }

        /// <summary>
        /// 驗證空購物車訊息
        /// </summary>
        public bool VerifyCartEmptyMessage(string expectedMessage)
        {
            return VerifyElementText(CartEmptyMessageId, expectedMessage);
        }

        /// <summary>
        /// 驗證結帳訊息
        /// </summary>
        public bool VerifyCheckoutMessage(string expectedMessage)
        {
            return VerifyElementText(CheckoutMessageId, expectedMessage);
        }

        /// <summary>
        /// 驗證結帳總金額
        /// </summary>
        public bool VerifyCheckoutTotal(string expectedTotal)
        {
            return VerifyElementText(CheckoutTotalId, expectedTotal);
        }

        #endregion
    }
}
