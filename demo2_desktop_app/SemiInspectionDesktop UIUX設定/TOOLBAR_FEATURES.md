# 工具列 Icon 與功能介紹（Inspection Data 對照）

> 圖示實作：`SemiInspectionDesktop/UI/ToolbarIcons.cs`  
> 提示文字：`SemiInspectionDesktop/UI/ToolbarFeatureGuide.cs`  
> 樣本資料：`Recipe_data/InspectionRecipe_Sample.json`

## Toolbar 0 — Recipe 操作

| 按鈕 | AutomationId | 快捷鍵 | Icon 語意 | 對應 Inspection 資料 |
|------|--------------|--------|-----------|----------------------|
| **Import Recipe** | `btnImportRecipe` | Ctrl+I | 晶圓 + Recipe 文件 + 匯入箭頭 | 完整 JSON Recipe：Lot/Wafer、OpticalMode、三大 Policy、DefectSummary |
| **Run Inspection** | `btnRunInspection` | Ctrl+R | AOI 掃描線 + 執行三角 | 讀取 ToolID、Illumination、Algorithm、Threshold；統計 DefectSummary |
| **About** | `btnToolbar0About` | — | 資訊圓圈 | 顯示功能與 Recipe 資料區段說明 |

### Import Recipe 功能介紹

將 `.json` Inspection Recipe 匯入 `Recipe_data` 並載入 Parameters 表。

**載入的資料區段：**

```
RecipeName, WaferID, LotID, Layer, Step, ToolID
OpticalMode      → Illumination, Aperture, PixelSize, FocusOffset, ScanSpeed
DetectionPolicy  → Algorithm, Threshold, SegmentBreak, SNRMin
ReportingPolicy  → MaxNuisanceRate, DefectClassSet, ReportEnabled
InspectionPolicy → ZoneOfInterest, RuleAction, MaxRetry
DefectSummary    → DefectType, DefectCount, DefectNumber, Severity, DOI
```

### Run Inspection 功能介紹

依目前已載入 Recipe 執行**模擬 AOI 檢測**，將關鍵參數與缺陷統計寫入日誌區 (`txtToolLog`)。

**輸出範例（對應 Sample Recipe）：**

- Recipe: `Layer1_AOI_Recipe_v1`
- Tool: `AOI-TOOL-03`
- Optical: `Brightfield / PixelSize=0.5`
- Detection: `PatternCompare / Threshold=45`
- DefectSummary 合計: 40 defects / 5 types

---

## Toolbar 1 — 檢視與分析

| 按鈕 | AutomationId | 快捷鍵 | Icon 語意 | 對應 Inspection 資料 |
|------|--------------|--------|-----------|----------------------|
| **Parameters** | `btnParameters` | Ctrl+E | 三色參數表（藍/橘/綠） | Category / Parameter / Value / Unit 展開表 |
| **Defect Chart** | `btnDefectChart` | Ctrl+D | 晶圓框 + 缺陷趨勢曲線 | DefectSummary 的 DefectType × DefectCount |

### Parameters Icon 色彩對照

| 色帶 | Policy 區段 | 典型參數 |
|------|-------------|----------|
| 藍 | OpticalMode | Illumination, Aperture, PixelSize |
| 橘 | DetectionPolicy | Algorithm, Threshold, SNRMin |
| 綠 | InspectionPolicy | ZoneOfInterest, RuleAction, MaxRetry |

### Defect Chart 功能介紹

以 `DefectSummary` 繪製曲線圖：

| 軸 | 欄位 | Sample 範例 |
|----|------|-------------|
| X | DefectType | Particle, Scratch, Bridge, Pattern, Void |
| Y | DefectCount | 18, 6, 3, 9, 4 |

---

## Icon 設計摘要

```
Import Recipe   [晶圓圓盤 + R 文件 + ↓ 箭頭]
Run Inspection  [═══ 掃描線 + ▶ 執行]
Parameters      [藍|橘|綠 三區參數表]
Defect Chart    [○ 晶圓 + 曲線 + ● 缺陷點]
About           [( i ) 資訊]
```

## 驗證

```bat
build_semi.bat
run_semi.bat
```

滑鼠移至工具列按鈕可看到 ToolTip；點 **About** 可查看完整功能與資料區段說明。
