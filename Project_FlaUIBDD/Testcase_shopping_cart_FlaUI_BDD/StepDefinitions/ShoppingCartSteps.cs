using NUnit.Framework;
using NUnit.Framework.Legacy;
using TechTalk.SpecFlow;
using ShoppingCartTests.PageObjects;
using ShoppingCartTests.Helpers;

namespace ShoppingCartTests.StepDefinitions
{
    [Binding]
    public class ShoppingCartSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private ProductListPage _productListPage;
        private ShoppingCartPage _shoppingCartPage;

        public ShoppingCartSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _productListPage = _scenarioContext.Get<ProductListPage>("ProductListPage");
            _shoppingCartPage = _scenarioContext.Get<ShoppingCartPage>("ShoppingCartPage");
        }

        #region Given Steps

        [Given(@"我已開啟購物網站")]
        public void Given我已開啟購物網站()
        {
            // 應用程式已在 Hooks 中啟動
            TestContext.WriteLine("購物網站已開啟");
        }

        [Given(@"購物車已清空")]
        public void Given購物車已清空()
        {
            _shoppingCartPage.ResetCart();
            TestContext.WriteLine("購物車已清空");
        }

        #endregion

        #region When Steps - 加入商品

        [When(@"我點擊加入蘋果按鈕")]
        public void When我點擊加入蘋果按鈕()
        {
            _productListPage.ClickAddApple();
            TestContext.WriteLine("已點擊加入蘋果按鈕");
        }

        [When(@"我點擊加入香蕉按鈕")]
        public void When我點擊加入香蕉按鈕()
        {
            _productListPage.ClickAddBanana();
            TestContext.WriteLine("已點擊加入香蕉按鈕");
        }

        [When(@"我點擊加入牛奶按鈕")]
        public void When我點擊加入牛奶按鈕()
        {
            _productListPage.ClickAddMilk();
            TestContext.WriteLine("已點擊加入牛奶按鈕");
        }

        [When(@"我點擊加入(.*)按鈕")]
        public void When我點擊加入按鈕(string itemName)
        {
            _productListPage.AddItem(itemName);
            TestContext.WriteLine($"已點擊加入{itemName}按鈕");
        }

        #endregion

        #region When Steps - 數量調整

        [When(@"我點擊(.*)的增加數量按鈕")]
        public void When我點擊增加數量按鈕(string itemName)
        {
            _shoppingCartPage.ClickIncreaseQuantity(itemName);
            TestContext.WriteLine($"已點擊{itemName}的增加數量按鈕");
        }

        [When(@"我點擊(.*)的減少數量按鈕")]
        public void When我點擊減少數量按鈕(string itemName)
        {
            _shoppingCartPage.ClickDecreaseQuantity(itemName);
            TestContext.WriteLine($"已點擊{itemName}的減少數量按鈕");
        }

        [When(@"我點擊移除(.*)按鈕")]
        public void When我點擊移除按鈕(string itemName)
        {
            _shoppingCartPage.ClickRemoveItem(itemName);
            TestContext.WriteLine($"已點擊移除{itemName}按鈕");
        }

        #endregion

        #region When Steps - 購物車操作

        [When(@"我點擊清空購物車按鈕")]
        public void When我點擊清空購物車按鈕()
        {
            _shoppingCartPage.ClickClearCart();
            TestContext.WriteLine("已點擊清空購物車按鈕");
        }

        [When(@"我點擊結帳按鈕")]
        public void When我點擊結帳按鈕()
        {
            _shoppingCartPage.ClickCheckout();
            TestContext.WriteLine("已點擊結帳按鈕");
        }

        [When(@"我關閉結帳視窗")]
        public void When我關閉結帳視窗()
        {
            _shoppingCartPage.CloseCheckoutModal();
            TestContext.WriteLine("已關閉結帳視窗");
        }

        #endregion

        #region Then Steps - 購物車驗證

        [Then(@"購物車件數應該是 ""(.*)""")]
        public void Then購物車件數應該是(string expectedCount)
        {
            var actualCount = _shoppingCartPage.GetCartCount();
            TestContext.WriteLine($"購物車件數: 預期={expectedCount}, 實際={actualCount}");
            ClassicAssert.AreEqual(expectedCount, actualCount,
                $"購物車件數不符: 預期 '{expectedCount}', 實際 '{actualCount}'");
        }

        [Then(@"購物車總計應該是 ""(.*)""")]
        public void Then購物車總計應該是(string expectedTotal)
        {
            var actualTotal = _shoppingCartPage.GetCartTotal();
            TestContext.WriteLine($"購物車總計: 預期={expectedTotal}, 實際={actualTotal}");
            ClassicAssert.AreEqual(expectedTotal, actualTotal,
                $"購物車總計不符: 預期 '{expectedTotal}', 實際 '{actualTotal}'");
        }

        [Then(@"購物車應該顯示空購物車訊息 ""(.*)""")]
        public void Then購物車應該顯示空購物車訊息(string expectedMessage)
        {
            var actualMessage = _shoppingCartPage.GetCartEmptyMessage();
            TestContext.WriteLine($"空購物車訊息: 預期={expectedMessage}, 實際={actualMessage}");
            ClassicAssert.AreEqual(expectedMessage, actualMessage,
                $"空購物車訊息不符: 預期 '{expectedMessage}', 實際 '{actualMessage}'");
        }

        #endregion

        #region Then Steps - 商品數量驗證

        [Then(@"蘋果數量應該是 ""(.*)""")]
        public void Then蘋果數量應該是(string expectedQuantity)
        {
            Then商品數量應該是("蘋果", expectedQuantity);
        }

        [Then(@"香蕉數量應該是 ""(.*)""")]
        public void Then香蕉數量應該是(string expectedQuantity)
        {
            Then商品數量應該是("香蕉", expectedQuantity);
        }

        [Then(@"牛奶數量應該是 ""(.*)""")]
        public void Then牛奶數量應該是(string expectedQuantity)
        {
            Then商品數量應該是("牛奶", expectedQuantity);
        }

        [Then(@"(.*)數量應該是 ""(.*)""")]
        public void Then商品數量應該是(string itemName, string expectedQuantity)
        {
            var actualQuantity = _shoppingCartPage.GetItemQuantity(itemName);
            TestContext.WriteLine($"{itemName}數量: 預期={expectedQuantity}, 實際={actualQuantity}");
            ClassicAssert.AreEqual(expectedQuantity, actualQuantity,
                $"{itemName}數量不符: 預期 '{expectedQuantity}', 實際 '{actualQuantity}'");
        }

        #endregion

        #region Then Steps - 結帳驗證

        [Then(@"結帳訊息應該是 ""(.*)""")]
        public void Then結帳訊息應該是(string expectedMessage)
        {
            var actualMessage = _shoppingCartPage.GetCheckoutMessage();
            TestContext.WriteLine($"結帳訊息: 預期={expectedMessage}, 實際={actualMessage}");
            ClassicAssert.AreEqual(expectedMessage, actualMessage,
                $"結帳訊息不符: 預期 '{expectedMessage}', 實際 '{actualMessage}'");
        }

        [Then(@"結帳總金額應該是 ""(.*)""")]
        public void Then結帳總金額應該是(string expectedTotal)
        {
            var actualTotal = _shoppingCartPage.GetCheckoutTotal();
            TestContext.WriteLine($"結帳總金額: 預期={expectedTotal}, 實際={actualTotal}");
            ClassicAssert.AreEqual(expectedTotal, actualTotal,
                $"結帳總金額不符: 預期 '{expectedTotal}', 實際 '{actualTotal}'");
        }

        #endregion
    }
}
