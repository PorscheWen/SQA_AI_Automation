# 執行 FlaUI BDD 測試並依 report_prompt.md 產生 Summary Report（需 Windows）
param(
    [string]$Configuration = "Release",
    [string]$Filter = ""
)

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$TestProject = Join-Path $ProjectRoot "Testcase_shopping_cart_FlaUI_BDD"
$ReportsDir = Join-Path $TestProject "reports"
$JUnitFile = Join-Path $ReportsDir "junit-results.xml"

if (-not (Test-Path $TestProject)) {
    throw "找不到測試專案：$TestProject"
}

New-Item -ItemType Directory -Path $ReportsDir -Force | Out-Null

Write-Host "=== FlaUI BDD：執行測試 ===" -ForegroundColor Green
Push-Location $TestProject
try {
    dotnet build -c $Configuration
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $testArgs = @(
        "test",
        "-c", $Configuration,
        "--no-build",
        "--logger", "junit;LogFilePath=$JUnitFile"
    )
    if ($Filter) {
        $testArgs += @("--filter", $Filter)
    }
    dotnet @testArgs
    $testExit = $LASTEXITCODE
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "=== 產生 Summary Report（report_prompt.md 格式）===" -ForegroundColor Green
& (Join-Path $ProjectRoot "generate_report.ps1") -JUnitPath "Testcase_shopping_cart_FlaUI_BDD\reports\junit-results.xml"
$reportExit = $LASTEXITCODE

Write-Host ""
Write-Host "報告位置：" -ForegroundColor Cyan
Write-Host "  $(Join-Path $ReportsDir 'summary_report.md')"
Write-Host "  $(Join-Path $ReportsDir 'summary_report.html')"
Write-Host "  $(Join-Path $ReportsDir 'junit-results.xml')"

if ($testExit -ne 0) { exit $testExit }
exit $reportExit
