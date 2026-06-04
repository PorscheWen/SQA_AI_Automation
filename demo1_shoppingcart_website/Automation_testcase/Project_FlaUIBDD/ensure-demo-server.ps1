# 確保 shopping_cart demo 網頁 (http://localhost:8888) 已啟動
param(
    [string]$Url = "http://localhost:8888/",
    [int]$WaitSeconds = 20
)

$ErrorActionPreference = "Stop"
$DemoDir = Join-Path $PSScriptRoot "..\demo\shopping_cart"
$ServeScript = Join-Path $DemoDir "serve.py"

if (-not (Test-Path $ServeScript)) {
    throw "找不到 $ServeScript"
}

function Get-PythonExe {
    if ($env:PYTHON_EXE -and (Test-Path $env:PYTHON_EXE)) { return $env:PYTHON_EXE }
    $py = Get-Command python -ErrorAction SilentlyContinue
    if ($py) { return $py.Source }
    $pyLauncher = Get-Command py -ErrorAction SilentlyContinue
    if ($pyLauncher) { return "$($pyLauncher.Source) -3" }
    throw "找不到 python，請安裝 Python 3 或設定 PYTHON_EXE"
}

function Test-DemoReady {
    foreach ($testUrl in @($Url, "http://127.0.0.1:8888/")) {
        try {
            $r = Invoke-WebRequest -Uri $testUrl -UseBasicParsing -TimeoutSec 2
            if ($r.StatusCode -eq 200) { return $true }
        } catch {
            continue
        }
    }
    return $false
}

if (Test-DemoReady) {
    Write-Host "Demo 網頁已在執行：$Url" -ForegroundColor Green
    return
}

Write-Host "啟動 Demo 網頁伺服器..." -ForegroundColor Cyan
$python = Get-PythonExe
if ($python -match "\s") {
    $proc = Start-Process -FilePath "py" -ArgumentList "-3", $ServeScript -WorkingDirectory $DemoDir -PassThru -WindowStyle Hidden
} else {
    $proc = Start-Process -FilePath $python -ArgumentList $ServeScript -WorkingDirectory $DemoDir -PassThru -WindowStyle Hidden
}

$deadline = (Get-Date).AddSeconds($WaitSeconds)
while ((Get-Date) -lt $deadline) {
    if (Test-DemoReady) {
        Write-Host "Demo 網頁已就緒：$Url (PID $($proc.Id))" -ForegroundColor Green
        return
    }
    Start-Sleep -Milliseconds 500
}

Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
throw "Demo 伺服器啟動逾時：$Url"
