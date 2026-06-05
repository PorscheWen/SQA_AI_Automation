@echo off
setlocal
cd /d "%~dp0"

echo [Demo2] Building solution (VS 2008 / .NET 3.5)...

set MSBUILD=
if exist "%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe" set "MSBUILD=%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
if exist "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" set "MSBUILD=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if exist "%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe" set "MSBUILD=%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe"
if exist "%WINDIR%\Microsoft.NET\Framework64\v3.5\MSBuild.exe" set "MSBUILD=%WINDIR%\Microsoft.NET\Framework64\v3.5\MSBuild.exe"

if "%MSBUILD%"=="" (
  echo ERROR: 找不到 MSBuild。請安裝 .NET Framework 3.5 或 Visual Studio 2008/較新版本。
  exit /b 1
)

"%MSBUILD%" Demo2Desktop.sln /p:Configuration=Debug /v:m
if errorlevel 1 exit /b 1

if not exist "Demo2Desktop\bin\Debug\Plugins" mkdir "Demo2Desktop\bin\Debug\Plugins"
copy /Y "Demo2Desktop.SamplePlugin\bin\Debug\Demo2Desktop.SamplePlugin.dll" "Demo2Desktop\bin\Debug\Plugins\" >nul 2>&1
if exist "Test_data" (
  if not exist "Demo2Desktop\bin\Debug\Test_data" mkdir "Demo2Desktop\bin\Debug\Test_data"
  xcopy /Y "Test_data\*.xlsx" "Demo2Desktop\bin\Debug\Test_data\" >nul 2>&1
  xcopy /Y "Test_data\*.json" "Demo2Desktop\bin\Debug\Test_data\" >nul 2>&1
)

echo.
echo Build OK: Demo2Desktop\bin\Debug\Demo2Desktop.exe
exit /b 0
