using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

namespace ShoppingCartTests.PageObjects
{
    /// <summary>
    /// 購物車頁面物件
    /// </summary>
    public class ShoppingCartPage : BasePage
    {
        // 元素定位器 (對應 HTML id，WebView2 會映射為 AutomationId)
        private const string CartCountId = "cart-count";
        private const string CartTotalId = "cart-total";
        private const string CartEmptyMessageId = "cart-empty-message";
        private const string ClearCartButtonId = "btn-clear-cart";
        private const string CheckoutButtonId = "btn-checkout";
        private const string CheckoutMessageId = "checkout-message";
        private const string CheckoutTotalId = "checkout-total";
        private const string CloseModalButtonId = "btn-close-modal";

        private const string QtyValuePrefix = "qty-value-";
        private const string QtyPlusPrefix = "btn-qty-plus-";
        private const string QtyMinusPrefix = "btn-qty-minus-";
        private const string RemoveItemPrefix = "btn-remove-";

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
            return GetCartCountText();
        }

        /// <summary>
        /// 取得購物車總計
        /// </summary>
        public string GetCartTotal()
        {
            var count = GetCartCount();
            if (count == "0 件")
            {
                return "NT$ 0";
            }

            if (TryGetElementText(CartTotalId, out var total) && !string.IsNullOrWhiteSpace(total))
            {
                return total;
            }

            return GetLastTextMatchingPattern(@"^NT\$ \d+$");
        }

        /// <summary>
        /// 取得空購物車訊息
        /// </summary>
        public string GetCartEmptyMessage()
        {
            if (TryGetElementText(CartEmptyMessageId, out var message))
            {
                return message;
            }

            return GetTextMatchingPattern(@"^購物車是空的");
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
            var productId = ResolveProductId(itemName);
            var automationId = $"{QtyValuePrefix}{productId}";

            if (TryGetElementText(automationId, out var quantity, 1000) && !string.IsNullOrWhiteSpace(quantity))
            {
                return quantity;
            }

            var found = Retry.WhileNull(
                () => FindQuantityFromQtyControls(productId) ?? FindQuantityFromCartItem(productId),
                TimeSpan.FromSeconds(8)
            ).Result;

            if (found != null)
            {
                return found;
            }

            throw new ElementNotAvailableException($"找不到商品數量: {itemName}");
        }

        private string FindQuantityFromQtyControls(string productId)
        {
            var plusButtonId = $"{QtyPlusPrefix}{productId}";
            var minusButtonId = $"{QtyMinusPrefix}{productId}";
            var anchor = FindElementInRoots(plusButtonId) ?? FindElementInRoots(minusButtonId);

            if (anchor?.Parent == null)
            {
                return null;
            }

            return FindDigitTextInContainer(anchor.Parent);
        }

        private string FindQuantityFromCartItem(string productId)
        {
            var cartItem = FindElementInRoots($"cart-item-{productId}");
            if (cartItem == null)
            {
                return null;
            }

            return FindDigitTextInContainer(cartItem);
        }

        private static string FindDigitTextInContainer(AutomationElement container)
        {
            foreach (var child in container.FindAllDescendants())
            {
                if (child.ControlType == ControlType.Button)
                {
                    continue;
                }

                var text = ReadElementText(child);
                if (IsDigitsOnly(text))
                {
                    return text;
                }
            }

            return null;
        }

        /// <summary>
        /// 點擊增加商品數量按鈕
        /// </summary>
        public void ClickIncreaseQuantity(string itemName)
        {
            var automationId = $"{QtyPlusPrefix}{ResolveProductId(itemName)}";
            ClickElement(automationId);
        }

        /// <summary>
        /// 點擊減少商品數量按鈕
        /// </summary>
        public void ClickDecreaseQuantity(string itemName)
        {
            var automationId = $"{QtyMinusPrefix}{ResolveProductId(itemName)}";
            ClickElement(automationId);
        }

        /// <summary>
        /// 點擊移除商品按鈕
        /// </summary>
        public void ClickRemoveItem(string itemName)
        {
            var automationId = $"{RemoveItemPrefix}{ResolveProductId(itemName)}";
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
            if (TryGetElementText(CheckoutMessageId, out var message, 3000) && !string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            return GetTextMatchingPattern(@"^感謝您的購買");
        }

        /// <summary>
        /// 取得結帳總金額
        /// </summary>
        public string GetCheckoutTotal()
        {
            if (TryGetElementText(CheckoutTotalId, out var total, 3000) && !string.IsNullOrWhiteSpace(total))
            {
                return total;
            }

            return GetTextMatchingPattern(@"^總金額：NT\$ \d+$");
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
            var automationId = $"{QtyValuePrefix}{ResolveProductId(itemName)}";
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

        private static string ResolveProductId(string itemName)
        {
            return itemName switch
            {
                "蘋果" => "apple",
                "香蕉" => "banana",
                "牛奶" => "milk",
                _ => itemName.ToLower()
            };
        }
    }
}
