# Capture Semi Inspection Desktop UI screenshots for operation manual (PrintWindow)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$imgDir = Join-Path $root 'manual_images'
$exe = Join-Path $root 'SemiInspectionDesktop\bin\Debug\SemiInspectionDesktop.exe'
$jsonSample = Join-Path $root 'Recipe_data\InspectionRecipe_Sample.json'

if (-not (Test-Path $exe)) {
    Write-Host '[INFO] Building app...'
    & (Join-Path $root 'build_semi.bat') | Out-Host
}

New-Item -ItemType Directory -Force -Path $imgDir | Out-Null
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$csharp = @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

public static class WinCap
{
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, int nFlags);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

    public static IntPtr FindByTitle(string titlePart)
    {
        IntPtr found = IntPtr.Zero;
        EnumWindows((hWnd, lParam) => {
            if (!IsWindowVisible(hWnd)) return true;
            var sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, sb.Capacity);
            if (sb.ToString().IndexOf(titlePart, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                found = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);
        return found;
    }

    public static void Capture(IntPtr hWnd, string path, string requiredTitlePart)
    {
        var sb = new StringBuilder(256);
        GetWindowText(hWnd, sb, sb.Capacity);
        if (sb.ToString().IndexOf(requiredTitlePart, StringComparison.OrdinalIgnoreCase) < 0)
            throw new Exception("Wrong window: " + sb.ToString());
        ShowWindow(hWnd, 9);
        SetForegroundWindow(hWnd);
        System.Threading.Thread.Sleep(500);
        RECT rect;
        GetWindowRect(hWnd, out rect);
        int w = rect.Right - rect.Left;
        int h = rect.Bottom - rect.Top;
        if (w <= 0 || h <= 0) throw new Exception("Invalid window size");
        using (var bmp = new Bitmap(w, h))
        using (var g = Graphics.FromImage(bmp))
        {
            IntPtr hdc = g.GetHdc();
            PrintWindow(hWnd, hdc, 0);
            g.ReleaseHdc(hdc);
            bmp.Save(path, ImageFormat.Png);
        }
    }
}
'@

Add-Type -TypeDefinition $csharp -ReferencedAssemblies System.Drawing, System.Windows.Forms

function Wait-AppWindow([string]$titlePart, [int]$timeoutSec = 25) {
    $deadline = (Get-Date).AddSeconds($timeoutSec)
    while ((Get-Date) -lt $deadline) {
        $h = [WinCap]::FindByTitle($titlePart)
        if ($h -ne [IntPtr]::Zero) { return $h }
        Start-Sleep -Milliseconds 400
    }
    throw "Window not found: $titlePart"
}

function Focus-App([IntPtr]$hwnd) {
    [WinCap]::SetForegroundWindow($hwnd) | Out-Null
    Start-Sleep -Milliseconds 500
}

function Send-AppKeys([string]$keys) {
    [System.Windows.Forms.SendKeys]::SendWait($keys)
}

$recipeSrc = Join-Path $root 'Recipe_data'
$recipeDst = Join-Path (Split-Path $exe) 'Recipe_data'
if (Test-Path $recipeSrc) {
    New-Item -ItemType Directory -Force -Path $recipeDst | Out-Null
    Copy-Item -Path (Join-Path $recipeSrc '*') -Destination $recipeDst -Recurse -Force -ErrorAction SilentlyContinue
}

Get-Process SemiInspectionDesktop -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1
Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe)

$title = 'Semi Inspection Desktop'
$hwnd = Wait-AppWindow $title
Start-Sleep -Seconds 2
[WinCap]::Capture($hwnd, (Join-Path $imgDir 'ui_01_main.png'), $title)
Write-Host 'Captured ui_01_main.png'

if (-not (Test-Path $jsonSample)) { throw "Missing sample: $jsonSample" }

Focus-App $hwnd
Send-AppKeys '^i'
Start-Sleep -Seconds 1
$dialogPath = (Resolve-Path $jsonSample).Path
Send-AppKeys $dialogPath
Start-Sleep -Milliseconds 300
Send-AppKeys '{ENTER}'
Start-Sleep -Seconds 2
[WinCap]::Capture($hwnd, (Join-Path $imgDir 'ui_02_import_recipe.png'), $title)
Write-Host 'Captured ui_02_import_recipe.png'

Focus-App $hwnd
Send-AppKeys '^e'
Start-Sleep -Seconds 1
[WinCap]::Capture($hwnd, (Join-Path $imgDir 'ui_03_parameters.png'), $title)
Write-Host 'Captured ui_03_parameters.png'

Focus-App $hwnd
Send-AppKeys '^d'
Start-Sleep -Seconds 2
[WinCap]::Capture($hwnd, (Join-Path $imgDir 'ui_04_chart.png'), $title)
Write-Host 'Captured ui_04_chart.png'

Write-Host "Done. Images in: $imgDir"
