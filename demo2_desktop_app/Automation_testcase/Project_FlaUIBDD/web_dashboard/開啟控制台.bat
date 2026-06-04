@echo off
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "PORT=6690"
set "URL=http://localhost:%PORT%/"
set "DEMO2_ROOT=%~dp0..\..\.."
set "BUILD_BAT=%DEMO2_ROOT%\build.bat"
set "EXE_PATH=%DEMO2_ROOT%\Demo2Desktop\bin\Debug\Demo2Desktop.exe"
set "FLAUIBDD_DASHBOARD_PORT=%PORT%"

echo.
echo Demo2 Desktop FlaUI BDD 測試控制台
echo   %URL%
echo   勾選 Features ^| 執行進度 ^| 測試結果
echo.
echo 按 Ctrl+C 停止伺服器。
echo.

if not exist "%EXE_PATH%" (
    echo 找不到 Demo2Desktop.exe，正在執行 build.bat...
    if not exist "%BUILD_BAT%" (
        echo 錯誤: 找不到 %BUILD_BAT%
        pause
        exit /b 1
    )
    call "%BUILD_BAT%"
    if errorlevel 1 (
        echo 錯誤: build.bat 建置失敗
        pause
        exit /b 1
    )
) else (
    echo Demo2Desktop 已就緒
)

if not exist "%~dp0server.py" (
    echo 錯誤: 找不到 server.py
    pause
    exit /b 1
)

start "" "%URL%"
python "%~dp0server.py"
set "EXIT_CODE=%ERRORLEVEL%"
if not "%EXIT_CODE%"=="0" (
    echo.
    echo 控制台啟動失敗 ^(exit %EXIT_CODE%^)
    pause
)
exit /b %EXIT_CODE%
