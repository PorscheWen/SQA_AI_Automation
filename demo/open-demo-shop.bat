@echo off
chcp 65001 >nul 2>&1
setlocal EnableExtensions
cd /d "%~dp0shopping_cart"
set "URL=http://localhost:8888/"

python -c "import urllib.request; urllib.request.urlopen(r'%URL%', timeout=2)" >nul 2>&1
if %ERRORLEVEL% equ 0 goto :open

echo 啟動 Demo Shop 伺服器...
start "Demo Shop Server" /MIN python serve.py

set /a WAIT=0
:wait_loop
python -c "import urllib.request; urllib.request.urlopen(r'%URL%', timeout=2)" >nul 2>&1
if %ERRORLEVEL% equ 0 goto :open
set /a WAIT+=1
if %WAIT% geq 40 (
    echo 錯誤：Demo Shop 伺服器啟動逾時（%URL%）
    exit /b 1
)
timeout /t 1 /nobreak >nul
goto :wait_loop

:open
start "" "%URL%"
echo.
echo Demo Shop 已開啟：%URL%
echo 伺服器視窗已最小化（標題 Demo Shop Server）。關閉該視窗即可停止服務。
echo.
endlocal
