# 依 report_prompt.md 從 JUnit XML 產生 TestComplete 格式 Summary Report
# 用法：
#   .\generate_report.ps1
#   .\generate_report.ps1 -JUnitPath "reports\junit-results.xml"

param(
    [string]$JUnitPath = "reports\junit-results.xml",
    [string]$SessionName = "Testcase_shopping_cart",
    [string]$ProjectName = "Project_Testcomplete"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$ReportsDir = Join-Path $ProjectRoot "reports"
$PromptRef = "report_prompt.md"
$ToolsScript = Join-Path $ProjectRoot "..\tools\generate_summary_report.py"

if (-not (Test-Path $ToolsScript)) {
    throw "找不到工具腳本：$ToolsScript"
}

if (-not (Test-Path $ReportsDir)) {
    New-Item -ItemType Directory -Path $ReportsDir | Out-Null
}

$junitFull = Join-Path $ProjectRoot $JUnitPath
if (-not (Test-Path $junitFull)) {
    Write-Host ""
    Write-Host "尚未找到 JUnit 檔案：$junitFull" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "請在 TestComplete 執行測試後匯出 Summary 為 JUnit：" -ForegroundColor Cyan
    Write-Host "  1. 開啟 Summary Report → Export Summary as JUnit" -ForegroundColor Cyan
    Write-Host "  2. 或於腳本結尾加入：Log.SaveResultsAs('$junitFull', lsJUnit)" -ForegroundColor Cyan
    Write-Host "  3. 或使用命令列：/ExportSummary:`"$junitFull`"" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "完成後再執行：.\generate_report.ps1" -ForegroundColor Cyan
    exit 1
}

python $ToolsScript `
    --junit $junitFull `
    --output-dir $ReportsDir `
    --session $SessionName `
    --project $ProjectName `
    --prompt-ref $PromptRef

exit $LASTEXITCODE
