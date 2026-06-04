@echo off
chcp 65001 >nul 2>&1
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "DEMO2_ROOT=%~dp0..\..\..\"
set "DEMO2_EXE=%DEMO2_ROOT%Demo2Desktop\bin\Debug\Demo2Desktop.exe"
set "INSPECTOR_EXE="

echo.
echo FlaUI Inspector + Demo2 Desktop
echo.

if defined FLAUI_INSPECTOR_PATH if exist "!FLAUI_INSPECTOR_PATH!" (
    set "INSPECTOR_EXE=!FLAUI_INSPECTOR_PATH!"
    goto :launch
)

for %%P in (
    "%~dp0tools\FlaUIInspector\FlaUIInspector.exe"
    "%~dp0FlaUIInspector\FlaUIInspector.exe"
    "%USERPROFILE%\FlaUIInspector\FlaUIInspector.exe"
    "%USERPROFILE%\Downloads\FlaUIInspector\FlaUIInspector.exe"
    "%LOCALAPPDATA%\FlaUIInspector\FlaUIInspector.exe"
    "%LOCALAPPDATA%\Programs\FlaUIInspector\FlaUIInspector.exe"
    "C:\Tools\FlaUIInspector\FlaUIInspector.exe"
) do (
    if exist %%~P (
        set "INSPECTOR_EXE=%%~P"
        goto :launch
    )
)

echo [ERROR] FlaUIInspector.exe not found.
echo.
echo Install one of the following:
echo   1. Extract FlaUIInspector to:
echo      tools\FlaUIInspector\FlaUIInspector.exe
echo   2. Set environment variable FLAUI_INSPECTOR_PATH
echo.
echo Download:
echo   https://github.com/FlaUI/FlaUIInspector/releases
echo.
echo Opening download page in browser...
start "" "https://github.com/FlaUI/FlaUIInspector/releases"
echo.
pause
exit /b 1

:launch
echo Using: %INSPECTOR_EXE%
echo.

tasklist /FI "IMAGENAME eq Demo2Desktop.exe" 2>nul | find /I "Demo2Desktop.exe" >nul
if errorlevel 1 (
    if exist "%DEMO2_EXE%" (
        echo Starting Demo2Desktop...
        start "" "%DEMO2_EXE%"
        timeout /t 2 /nobreak >nul
    ) else (
        echo Warning: Demo2Desktop.exe not found.
        echo Run demo2_desktop_app\build.bat first.
    )
) else (
    echo Demo2Desktop is already running.
)

echo Starting FlaUI Inspector...
start "" "%INSPECTOR_EXE%"
echo.
echo In Inspector, select process Demo2Desktop.
echo Check controls: treeFiles, dataGridExcel, btnToolbar0ImportExcel
echo.
exit /b 0
