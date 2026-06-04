using System;
using System.IO;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using System.Drawing.Imaging;

namespace ShoppingCartTests.Helpers
{
    /// <summary>
    /// 截圖輔助類別
    /// </summary>
    public static class ScreenshotHelper
    {
        /// <summary>
        /// 對視窗截圖並儲存
        /// </summary>
        public static void TakeScreenshot(Window window, string filePath)
        {
            try
            {
                if (window == null)
                {
                    throw new ArgumentNullException(nameof(window));
                }

                var captureImage = window.Capture();
                captureImage.Save(filePath, ImageFormat.Png);
                Console.WriteLine($"截圖已儲存: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"截圖失敗: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 對元素截圖並儲存
        /// </summary>
        public static void TakeElementScreenshot(AutomationElement element, string filePath)
        {
            try
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                var captureImage = element.Capture();
                captureImage.Save(filePath, ImageFormat.Png);
                Console.WriteLine($"元素截圖已儲存: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"元素截圖失敗: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 產生截圖檔案名稱
        /// </summary>
        public static string GenerateFileName(string prefix, string extension = "png")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{prefix}_{timestamp}.{extension}";
        }

        /// <summary>
        /// 確保截圖目錄存在
        /// </summary>
        public static string EnsureScreenshotDirectory(string baseDir = "Screenshots")
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), baseDir);
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }
    }
}
