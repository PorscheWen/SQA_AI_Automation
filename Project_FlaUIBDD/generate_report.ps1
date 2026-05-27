# 依 report_prompt.md 從 JUnit XML 產生 TestComplete 格式 Summary Report
# 用法：
#   .\generate_report.ps1
#   .\generate_report.ps1 -JUnitPath "Testcase_shopping_cart_FlaUI_BDD\reports\junit-results.xml"

param(
    [string]$JUnitPath = "Testcase_shopping_cart_FlaUI_BDD\reports\junit-results.xml",
    [string]$SessionName = "Testcase_shopping_cart_FlaUI_BDD",
    [string]$ProjectName = "Project_FlaUIBDD"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$PromptRef = "../Project_Testcomplete/report_prompt.md"
$ToolsScript = Join-Path $ProjectRoot "..\tools\generate_summary_report.py"

if (-not (Test-Path $ToolsScript)) {
    throw "找不到工具腳本：$ToolsScript"
}

$junitFull = Join-Path $ProjectRoot $JUnitPath
$reportsDir = Split-Path $junitFull -Parent

if (-not (Test-Path $junitFull)) {
    Write-Host ""
    Write-Host "尚未找到 JUnit 檔案：$junitFull" -ForegroundColor Yellow
    Write-Host "請先執行：.\run-tests-and-report.ps1" -ForegroundColor Cyan
    Write-Host "或手動：cd Testcase_shopping_cart_FlaUI_BDD && dotnet test --logger `"junit;LogFilePath=reports\junit-results.xml`"" -ForegroundColor Cyan
    exit 1
}

if (-not (Test-Path $reportsDir)) {
    New-Item -ItemType Directory -Path $reportsDir | Out-Null
}

python $ToolsScript `
    --junit $junitFull `
    --output-dir $reportsDir `
    --session $SessionName `
    --project $ProjectName `
    --prompt-ref $PromptRef

exit $LASTEXITCODE
