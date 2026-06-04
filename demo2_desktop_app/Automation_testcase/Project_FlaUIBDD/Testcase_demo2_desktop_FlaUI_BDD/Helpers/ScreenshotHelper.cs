using System.Drawing;
using System.Drawing.Imaging;
using FlaUI.Core.AutomationElements;

namespace Demo2DesktopTests.Helpers;

public static class ScreenshotHelper
{
    public static void TakeScreenshot(Window window, string filePath)
    {
        var captureImage = window.Capture();
        captureImage.Save(filePath, ImageFormat.Png);
        Console.WriteLine($"截圖已儲存: {filePath}");
    }

    public static void CaptureDesktop(string filePath)
    {
        var bounds = System.Windows.Forms.Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
        }

        bitmap.Save(filePath, ImageFormat.Png);
        Console.WriteLine($"桌面截圖已儲存: {filePath}");
    }
}
