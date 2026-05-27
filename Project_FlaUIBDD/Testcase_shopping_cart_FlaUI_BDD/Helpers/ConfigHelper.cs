using System;
using System.Configuration;

namespace ShoppingCartTests.Helpers
{
    /// <summary>
    /// 配置管理輔助類別
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 取得應用程式路徑
        /// </summary>
        public static string GetApplicationPath()
        {
            return GetConfigValue("ApplicationPath", @"C:\Path\To\Application.exe");
        }

        /// <summary>
        /// 取得應用程式 URL
        /// </summary>
        public static string GetApplicationUrl()
        {
            return GetConfigValue("ApplicationUrl", "http://localhost:8080");
        }

        /// <summary>
        /// 取得預設超時時間（毫秒）
        /// </summary>
        public static int GetDefaultTimeout()
        {
            var timeout = GetConfigValue("DefaultTimeout", "10000");
            return int.TryParse(timeout, out var result) ? result : 10000;
        }

        /// <summary>
        /// 取得截圖目錄
        /// </summary>
        public static string GetScreenshotDirectory()
        {
            return GetConfigValue("ScreenshotDirectory", "Screenshots");
        }

        /// <summary>
        /// 是否在測試失敗時截圖
        /// </summary>
        public static bool TakeScreenshotOnFailure()
        {
            var value = GetConfigValue("TakeScreenshotOnFailure", "true");
            return bool.TryParse(value, out var result) && result;
        }

        /// <summary>
        /// 取得瀏覽器類型（如果使用 Web 應用）
        /// </summary>
        public static string GetBrowserType()
        {
            return GetConfigValue("BrowserType", "Chrome");
        }

        /// <summary>
        /// 取得配置值，若不存在則返回預設值
        /// </summary>
        private static string GetConfigValue(string key, string defaultValue)
        {
            try
            {
                // 優先從環境變數讀取
                var envValue = Environment.GetEnvironmentVariable(key);
                if (!string.IsNullOrEmpty(envValue))
                {
                    return envValue;
                }

                // 從 App.config 讀取
                var configValue = ConfigurationManager.AppSettings[key];
                return !string.IsNullOrEmpty(configValue) ? configValue : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
