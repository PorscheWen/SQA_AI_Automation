# 啟動 FlaUI BDD 測試控制台（三 Tab 網頁）
# 注意：Chrome/Edge 封鎖 6665-6669，無法使用 http://localhost:6666/（ERR_UNSAFE_PORT）
param(
    [int]$Port = 6688,
    [switch]$OpenBrowser = $true
)

$ErrorActionPreference = "Stop"
$DashboardDir = Join-Path $PSScriptRoot "web_dashboard"
$ServerScript = Join-Path $DashboardDir "server.py"

if (-not (Test-Path $ServerScript)) {
    Write-Host "錯誤: 找不到 $ServerScript" -ForegroundColor Red
    exit 1
}

$env:FLAUIBDD_DASHBOARD_PORT = $Port
$url = "http://localhost:$Port/"

Write-Host ""
Write-Host "FlaUI BDD 測試控制台" -ForegroundColor Cyan
Write-Host "  $url" -ForegroundColor White
Write-Host '  勾選 Features | 執行進度 | 測試結果' -ForegroundColor Gray
Write-Host ""
Write-Host "按 Ctrl+C 停止伺服器。" -ForegroundColor Yellow
Write-Host ""

Write-Host "啟動 Demo 購物車網頁 (localhost:8888)..." -ForegroundColor Green
& (Join-Path $PSScriptRoot "ensure-demo-server.ps1")

if ($OpenBrowser) {
    Start-Process $url
}

Push-Location $DashboardDir
try {
    python $ServerScript
}
finally {
    Pop-Location
}
