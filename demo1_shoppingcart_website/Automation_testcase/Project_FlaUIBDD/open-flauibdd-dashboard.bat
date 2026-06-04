@echo off
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "PORT=6688"
set "URL=http://localhost:%PORT%/"
set "DEMO_URL=http://localhost:8888/"
set "DEMO_DIR=%~dp0..\demo\shopping_cart"
set "FLAUIBDD_DASHBOARD_PORT=%PORT%"

echo.
echo FlaUI BDD Dashboard
echo   %URL%
echo   Tabs: Features ^| Progress ^| Results
echo.
echo Press Ctrl+C to stop the server.
echo.

python -c "import urllib.request; urllib.request.urlopen(r'%DEMO_URL%', timeout=2)" >nul 2>&1
if errorlevel 1 (
    echo Starting Demo Shop ^(port 8888^)...
    if not exist "%DEMO_DIR%\serve.py" (
        echo ERROR: serve.py not found in %DEMO_DIR%
        pause
        exit /b 1
    )
    start "Demo Shop Server" /MIN cmd /c "pushd /d \"%DEMO_DIR%\" && python serve.py"
    for /l %%i in (1,1,40) do (
        python -c "import urllib.request; urllib.request.urlopen(r'%DEMO_URL%', timeout=2)" >nul 2>&1
        if not errorlevel 1 goto :demo_ready
        timeout /t 1 /nobreak >nul
    )
    echo ERROR: Demo Shop server start timeout
    pause
    exit /b 1
) else (
    echo Demo Shop already running: %DEMO_URL%
)

:demo_ready
start "" "%URL%"

if not exist "%~dp0web_dashboard\server.py" (
    echo ERROR: web_dashboard\server.py not found
    pause
    exit /b 1
)

cd /d "%~dp0web_dashboard"
python server.py
set "EXIT_CODE=%ERRORLEVEL%"
if not "%EXIT_CODE%"=="0" (
    echo.
    echo Dashboard failed to start ^(exit %EXIT_CODE%^).
    pause
)
exit /b %EXIT_CODE%
