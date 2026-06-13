# Semi Inspection Desktop App

半導體 AOI Inspection Recipe 檢視桌面程式（.NET 3.5 WinForms），UI/UX 延續原 Demo2 版型。

## 建置與執行

| 批次檔 | 用途 |
|--------|------|
| `build_semi.bat` | 建置 `SemiInspectionDesktop.sln` |
| `run_semi.bat` | 啟動 `SemiInspectionDesktop.exe`（未建置時自動建置） |

```bat
build_semi.bat
run_semi.bat
```

產出：`SemiInspectionDesktop\bin\Debug\SemiInspectionDesktop.exe`

## 資料與文件

| 路徑 | 說明 |
|------|------|
| `Recipe_data/` | Inspection Recipe JSON（含 `InspectionRecipe_Sample.json`） |
| `SemiInspectionDesktop UIUX設定/` | 參數說明、UI 版型、樣本 JSON |
| `操作app說明書.html` | 操作說明（瀏覽器） |
| `開啟操作app說明書.bat` | 以系統瀏覽器開啟說明書 |

## 主要功能

- **Import Recipe** (Ctrl+I) — 匯入 JSON 至 Recipe_data
- **RawData** (Ctrl+E) — 檢視 Lot/Wafer、OpticalMode、三大 Policy
- **Defect Chart** (Ctrl+D) — DefectType / DefectCount 曲線圖
- **Run Inspection** (Ctrl+R) — 模擬檢測 log
- **Close Recipe** (Ctrl+W) — 關閉目前 Recipe

## Visual Studio

開啟 `SemiInspectionDesktop.sln`（VS 2008 / .NET 3.5）。
