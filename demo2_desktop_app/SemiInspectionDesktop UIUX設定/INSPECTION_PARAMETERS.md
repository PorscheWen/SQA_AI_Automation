# 半導體 Inspection 所需參數

> UI/UX 版型參考同層 `uiux_layout.json` 與 `../SemiInspectionDesktop` 主畫面配置。

## 1. Recipe 三大 Policy（Camtek / KLA 類 AOI 通用）

| Policy | 用途 | 典型參數 |
|--------|------|----------|
| **Detection Policy** | 缺陷偵測演算法與靈敏度 | Algorithm, Threshold, SegmentBreak, SNRMin |
| **Reporting Policy** | 缺陷分類與報告 | DefectClassSet, MaxNuisanceRate, ReportEnabled |
| **Inspection Policy** | 掃描行為與區域 | ZoneOfInterest, RuleAction, MaxRetry |

## 2. 光學模式（Optical Mode）

| 參數 | 說明 | 單位 |
|------|------|------|
| Illumination | 照明模式（Brightfield / Darkfield） | - |
| Aperture | 孔徑 / NA | - |
| PixelSize | 像素尺寸 | um |
| FocusOffset | 焦點偏移 | um |
| ScanSpeed | 掃描速度 | mm/s |

## 3. Lot / Wafer 識別

| 參數 | 說明 |
|------|------|
| RecipeName | 檢測配方名稱 |
| WaferID | 晶圓編號 |
| LotID | 批次編號 |
| Layer | 製程層別 |
| Step | 製程步驟 |
| ToolID | 機台編號 |

## 4. 缺陷摘要（Defect Summary）

| 欄位 | 說明 |
|------|------|
| DefectType | 缺陷類型（Particle, Scratch, Bridge, Pattern…） |
| DefectCount | 該類型數量 |
| DefectNumber | 代表缺陷編號 |
| Severity | Critical / Major / Minor |
| DOI | Defect of Interest 標記 |

## 5. 工具列 Icon 與功能

詳見同層 [`TOOLBAR_FEATURES.md`](TOOLBAR_FEATURES.md) 與 `uiux_layout.json` 的 `toolbarFeatures`。

| 功能 | 說明 |
|------|------|
| Import Recipe | 匯入 JSON Recipe 至 Recipe_data |
| Run Inspection | 依 Recipe 模擬檢測 log |
| Parameters | 顯示 Category / Parameter / Value / Unit |
| Defect Chart | X=DefectType, Y=DefectCount |
| Close Recipe | 關閉目前 Recipe 與圖表 |

樣本配方：`Recipe_data/InspectionRecipe_Sample.json`
