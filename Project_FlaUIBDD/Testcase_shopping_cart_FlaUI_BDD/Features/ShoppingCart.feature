Feature: 購物車功能測試
  作為一個使用者
  我想要測試購物車功能
  以確保商品管理運作正常

@ShoppingCart @AddItem
Scenario: TC01 - 加入單一商品
  Given 購物車已清空
  When 我點擊加入蘋果按鈕
  Then 購物車件數應該是 "1 件"
  And 購物車總計應該是 "NT$ 30"
  And 蘋果數量應該是 "1"

@ShoppingCart @AddItem
Scenario: TC02 - 加入多種商品
  Given 購物車已清空
  When 我點擊加入蘋果按鈕
  And 我點擊加入香蕉按鈕
  And 我點擊加入牛奶按鈕
  Then 購物車件數應該是 "3 件"
  And 購物車總計應該是 "NT$ 105"

@ShoppingCart @Quantity
Scenario: TC03 - 增加購物車商品數量
  Given 購物車已清空
  When 我點擊加入香蕉按鈕
  And 我點擊香蕉的增加數量按鈕
  Then 香蕉數量應該是 "2"
  And 購物車件數應該是 "2 件"
  And 購物車總計應該是 "NT$ 40"

@ShoppingCart @RemoveItem
Scenario: TC04 - 從購物車移除商品
  Given 購物車已清空
  When 我點擊加入牛奶按鈕
  And 我點擊移除牛奶按鈕
  Then 購物車件數應該是 "0 件"
  And 購物車應該顯示空購物車訊息 "購物車是空的，請從左側加入商品。"

@ShoppingCart @ClearCart
Scenario: TC05 - 清空購物車
  Given 購物車已清空
  When 我點擊加入蘋果按鈕
  And 我點擊加入牛奶按鈕
  And 我點擊清空購物車按鈕
  Then 購物車件數應該是 "0 件"
  And 購物車總計應該是 "NT$ 0"

@ShoppingCart @Checkout
Scenario: TC06 - 測試結帳流程
  Given 購物車已清空
  When 我點擊加入蘋果按鈕
  And 我點擊加入香蕉按鈕
  And 我點擊結帳按鈕
  Then 結帳訊息應該是 "感謝您的購買！訂單已成立。"
  And 結帳總金額應該是 "總金額：NT$ 50"
  When 我關閉結帳視窗
  Then 購物車件數應該是 "0 件"

@ShoppingCart @AddItem @Parameterized
Scenario Outline: TC07 - 驗證不同商品的加入功能
  Given 購物車已清空
  When 我點擊加入<商品>按鈕
  Then 購物車件數應該是 "1 件"
  And 購物車總計應該是 "<總計>"

  Examples:
    | 商品 | 總計     |
    | 蘋果 | NT$ 30  |
    | 香蕉 | NT$ 20  |
    | 牛奶 | NT$ 55  |
