@echo off
setlocal
cd /d "%~dp0"

set EXE=Demo2Desktop\bin\Debug\Demo2Desktop.exe

if not exist "%EXE%" (
  echo [Demo2] 尚未建置，先執行 build.bat ...
  call "%~dp0build.bat"
  if errorlevel 1 exit /b 1
)

if not exist "Demo2Desktop\bin\Debug\Plugins" mkdir "Demo2Desktop\bin\Debug\Plugins"
if exist "Demo2Desktop.SamplePlugin\bin\Debug\Demo2Desktop.SamplePlugin.dll" (
  copy /Y "Demo2Desktop.SamplePlugin\bin\Debug\Demo2Desktop.SamplePlugin.dll" "Demo2Desktop\bin\Debug\Plugins\" >nul
)
if exist "Test_data" (
  if not exist "Demo2Desktop\bin\Debug\Test_data" mkdir "Demo2Desktop\bin\Debug\Test_data"
  xcopy /Y "Test_data\*.xlsx" "Demo2Desktop\bin\Debug\Test_data\" >nul 2>&1
  xcopy /Y "Test_data\*.json" "Demo2Desktop\bin\Debug\Test_data\" >nul 2>&1
)

start "" "%CD%\%EXE%"
echo Started: %CD%\%EXE%
exit /b 0
