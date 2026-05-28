# FlaUI BDD 測試與 HTML 報告生成腳本
# 執行測試並自動生成 HTML 報告

param(
    [string]$Configuration = "Debug",
    [string]$Filter = "",
    [switch]$OpenReport = $false
)

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$TestProject = Join-Path $ProjectRoot "Testcase_shopping_cart_FlaUI_BDD"
$ReportsDir = Join-Path $TestProject "reports"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  FlaUI BDD 測試 & HTML 報告生成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 確保測試專案存在
if (-not (Test-Path $TestProject)) {
    Write-Host "錯誤: 找不到測試專案: $TestProject" -ForegroundColor Red
    exit 1
}

# 建立報告目錄
New-Item -ItemType Directory -Path $ReportsDir -Force | Out-Null

# 步驟 0: 啟動 shopping_cart demo 網頁 (localhost:8888)
Write-Host "步驟 0: 啟動 Demo 購物車網頁..." -ForegroundColor Green
& (Join-Path $ProjectRoot "ensure-demo-server.ps1")
Write-Host ""

# 步驟 1: 還原 NuGet 套件
Write-Host "步驟 1/4: 還原 NuGet 套件..." -ForegroundColor Green
Push-Location $TestProject
try {
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "NuGet 套件還原失敗" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}
finally {
    Pop-Location
}
Write-Host "✓ NuGet 套件還原完成" -ForegroundColor Green
Write-Host ""

# 步驟 2: 建置專案
Write-Host "步驟 2/4: 建置測試專案..." -ForegroundColor Green
Push-Location $TestProject
try {
    dotnet build -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Host "專案建置失敗" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}
finally {
    Pop-Location
}
Write-Host "✓ 專案建置完成" -ForegroundColor Green
Write-Host ""

# 步驟 3: 執行測試
Write-Host "步驟 3/4: 執行測試..." -ForegroundColor Green
Write-Host "註: 測試期間會自動生成 HTML 報告" -ForegroundColor Yellow
Write-Host ""

Push-Location $TestProject
try {
    $testArgs = @(
        "test",
        "-c", $Configuration,
        "--no-build",
        "--logger", "console;verbosity=normal"
    )
    
    if ($Filter) {
        $testArgs += @("--filter", $Filter)
        Write-Host "使用測試篩選: $Filter" -ForegroundColor Yellow
    }
    
    dotnet @testArgs
    $testExitCode = $LASTEXITCODE
}
finally {
    Pop-Location
}

Write-Host ""
if ($testExitCode -eq 0) {
    Write-Host "✓ 測試執行完成 - 全部通過" -ForegroundColor Green
} else {
    Write-Host "⚠ 測試執行完成 - 有測試失敗" -ForegroundColor Yellow
}
Write-Host ""

# 步驟 4: 檢查報告
Write-Host "步驟 4/4: 檢查生成的報告..." -ForegroundColor Green

$htmlReport = Join-Path $ReportsDir "TestReport.html"
$junitReport = Join-Path $ReportsDir "junit-results.xml"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  報告檔案位置" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if (Test-Path $htmlReport) {
    Write-Host "✓ HTML 報告: " -ForegroundColor Green -NoNewline
    Write-Host "$htmlReport" -ForegroundColor White
} else {
    Write-Host "⚠ HTML 報告尚未生成" -ForegroundColor Yellow
}

if (Test-Path $junitReport) {
    Write-Host "✓ JUnit XML: " -ForegroundColor Green -NoNewline
    Write-Host "$junitReport" -ForegroundColor White
}

$screenshotDir = Join-Path $ReportsDir "screenshots"
if (Test-Path $screenshotDir) {
    $screenshots = Get-ChildItem $screenshotDir -Filter *.png
    if ($screenshots.Count -gt 0) {
        Write-Host "✓ 截圖目錄: " -ForegroundColor Green -NoNewline
        Write-Host "$screenshotDir ($($screenshots.Count) 個截圖)" -ForegroundColor White
    }
}

Write-Host ""

# 自動開啟報告（可選）
if ($OpenReport -and (Test-Path $htmlReport)) {
    Write-Host "正在開啟 HTML 報告..." -ForegroundColor Cyan
    Start-Process $htmlReport
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  完成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not $OpenReport) {
    Write-Host "提示: 使用 -OpenReport 參數可自動開啟報告" -ForegroundColor Gray
    Write-Host "範例: .\run-tests-html.ps1 -OpenReport" -ForegroundColor Gray
    Write-Host ""
}

exit $testExitCode
