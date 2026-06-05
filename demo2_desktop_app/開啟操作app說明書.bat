@echo off
chcp 65001 >nul 2>&1
setlocal EnableExtensions
cd /d "%~dp0"

echo ========================================
echo   Demo2 Desktop App - 操作說明書
echo ========================================
echo.

set "GUIDE_HTML=%~dp0操作app說明書.html"
set "GUIDE_XML=%~dp0操作app說明書.xml"

if exist "%GUIDE_HTML%" goto :open_html

echo [WARN] 找不到 HTML: %GUIDE_HTML%
if not exist "%GUIDE_XML%" (
    echo [ERROR] 也找不到 XML: %GUIDE_XML%
    pause
    exit /b 1
)

echo [INFO] 改以 XML 開啟（可能只顯示原始 XML 樹狀結構）
set "GUIDE_HTML=%GUIDE_XML%"
goto :open_html

:open_html
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "Start-Process -FilePath '%GUIDE_HTML%'"
if errorlevel 1 (
    echo [ERROR] 無法啟動瀏覽器，請手動開啟:
    echo %GUIDE_HTML%
    pause
    exit /b 1
)

echo 已開啟: %GUIDE_HTML%
echo.
pause
exit /b 0
