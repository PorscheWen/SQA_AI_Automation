Feature: Demo2 桌面應用程式測試
  作為測試人員
  我想要驗證 Demo2 Desktop App 的檔案、資料表與圖表功能
  以確保 Test_data 工作流程正常

@Functional @Import
Scenario: TC01 - Import Excel 至 Test_data
  Given 測試資料已就緒
  And 應用程式已重新啟動
  When 我點擊工具列「Import Excel」
  And 我在檔案對話框選擇樣本 X.xlsx
  Then Test_data 應存在 X.xlsx
  And 資料表應可見
  And 日誌區應包含「Import Excel」

@Functional @FileTree
Scenario: TC02 - File Tree 顯示 Test_data
  Given 測試資料已就緒
  And 應用程式已啟動
  Then 主視窗標題應為「Demo2 Desktop App」
  And 檔案樹應可見
  And Test_data 應存在 X.xlsx

@Functional @DataTable
Scenario: TC03 - Icon1 切換 Data Table
  Given 測試資料已就緒
  And 應用程式已啟動
  When 我點擊工具列「Data Table」
  Then 資料表應可見
  And 日誌區應包含「Data Table」

@Functional @Chart
Scenario: TC04 - Icon2 繪製曲線圖
  Given 測試資料已就緒
  And 應用程式已啟動
  When 我點擊工具列「Data Table」
  And 我點擊工具列「Draw data」
  Then 日誌區應包含「Draw data」

@Functional @FileTree
Scenario: TC05 - 檔案樹雙擊開啟 Excel
  Given 測試資料已就緒
  And 應用程式已啟動
  When 我在檔案樹雙擊 X.xlsx 或 Demo2_10 測試檔
  Then 資料表應可見
  And 日誌區應包含「開啟」

@Functional @About
Scenario: TC06 - About 對話框
  Given 應用程式已啟動
  When 我點擊工具列「About」
  And 我關閉訊息對話框

@Negative
Scenario: TC07 - 匯入非 Excel 檔
  Given 測試資料已就緒
  And 應用程式已啟動
  When 我點擊工具列「Import Excel」
  And 我在檔案對話框選擇無效檔 _invalid_sample.txt
  Then 不應將無效檔複製為 TC01_import_copy.xlsx
  And 我關閉訊息對話框

@Negative @Chart
Scenario: TC08 - 無資料時繪圖
  Given 應用程式已重新啟動
  When 我點擊工具列「Draw data」
  Then 主視窗仍應存在

@Negative
Scenario: TC09 - 開啟不存在檔案
  Given 應用程式已啟動
  When 我使用快捷鍵開啟 Data Table 並選擇不存在檔 not_exist_99999.xlsx
  Then 主視窗仍應存在

@Compatibility
Scenario: TC10 - xlsx 格式相容
  Given 測試資料已就緒
  And 應用程式已啟動
  When 我點擊工具列「Data Table」
  Then 資料表應可見
  And 日誌區應包含「Excel」
