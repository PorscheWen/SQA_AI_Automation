Feature: 購物車功能測試
  作為一個使用者
  我想要測試購物車功能
  以確保商品管理運作正常

@ShoppingCart @AddItem
Scenario: 加入單一商品
  Given 購物車已清空
  When 我點擊加入蘋果按鈕
  Then 購物車件數應該是 "1 件"
  And 購物車總計應該是 "NT$ 30"
