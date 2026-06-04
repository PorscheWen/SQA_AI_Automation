@echo off
REM FlaUI BDD 測試框架使用說明書
REM 使用預設瀏覽器開啟使用說明書

echo ========================================
echo   開啟 FlaUI BDD 使用說明書
echo ========================================
echo.

REM 取得批次檔所在目錄
set SCRIPT_DIR=%~dp0

REM 使用預設瀏覽器開啟 HTML 檔案
start "" "%SCRIPT_DIR%使用說明書.html"

echo 使用說明書已在瀏覽器中開啟！
echo.
pause
