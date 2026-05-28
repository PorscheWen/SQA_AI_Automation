using System;
using System.Configuration;
using System.IO;
using System.Reflection;

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
            return GetConfigValue("ApplicationUrl", "http://localhost:8888");
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
        /// 取得失敗 log 目錄
        /// </summary>
        public static string GetFailureLogDirectory()
        {
            return GetConfigValue("FailureLogDirectory", "FailureLogs");
        }

        /// <summary>
        /// 是否在測試失敗時寫入診斷 log
        /// </summary>
        public static bool WriteFailureLogOnFailure()
        {
            var value = GetConfigValue("WriteFailureLogOnFailure", "true");
            return bool.TryParse(value, out var result) && result;
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
        /// 測試前是否自動啟動 demo/shopping_cart/serve.py
        /// </summary>
        public static bool AutoStartDemoServer()
        {
            var value = GetConfigValue("AutoStartDemoServer", "true");
            return !bool.TryParse(value, out var result) || result;
        }

        /// <summary>
        /// demo 伺服器腳本路徑（可選，空白則自動搜尋）
        /// </summary>
        public static string GetDemoServerScriptPath()
        {
            return GetConfigValue("DemoServerScript", "");
        }

        /// <summary>
        /// 取得配置值，若不存在則返回預設值
        /// </summary>
        private static string GetConfigValue(string key, string defaultValue)
        {
            try
            {
                var envValue = Environment.GetEnvironmentVariable(key);
                if (!string.IsNullOrEmpty(envValue))
                {
                    return envValue;
                }

                var configValue = GetAppSettings()[key]?.Value;
                return !string.IsNullOrEmpty(configValue) ? configValue : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static KeyValueConfigurationCollection GetAppSettings()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var configFile = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.dll.config");

            if (!File.Exists(configFile))
            {
                configFile = Path.Combine(AppContext.BaseDirectory, "App.config");
            }

            var configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile
            };

            return ConfigurationManager
                .OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None)
                .AppSettings
                .Settings;
        }
    }
}
