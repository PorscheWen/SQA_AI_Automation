using System.Configuration;
using System.IO;
using System.Reflection;

namespace Demo2DesktopTests.Helpers;

public static class ConfigHelper
{
    public static string GetApplicationPath() =>
        GetConfigValue("ApplicationPath", @"C:\Path\To\Demo2Desktop.exe");

    public static string GetApplicationTitle() =>
        GetConfigValue("ApplicationTitle", "Demo2 Desktop App");

    public static string GetProcessName() =>
        GetConfigValue("ProcessName", "Demo2Desktop");

    public static string GetTestDataDirectory() =>
        GetConfigValue("TestDataDirectory", @"C:\Test_data");

    public static string GetSampleXlsxPath() =>
        Path.Combine(GetTestDataDirectory(), GetConfigValue("SampleXlsx", "X.xlsx"));

    public static string GetSampleJsonPath() =>
        Path.Combine(GetTestDataDirectory(), GetConfigValue("SampleJson", "TestType_Defect.json"));

    public static string GetImportTargetPath() =>
        Path.Combine(GetTestDataDirectory(), GetConfigValue("ImportTargetFile", "TC01_import_copy.json"));

    public static string GetInvalidSamplePath() =>
        Path.Combine(GetTestDataDirectory(), GetConfigValue("InvalidSampleFile", "_invalid_sample.txt"));

    public static int GetDefaultTimeout()
    {
        var timeout = GetConfigValue("DefaultTimeout", "30000");
        return int.TryParse(timeout, out var result) ? result : 30000;
    }

    public static string GetScreenshotDirectory() =>
        GetConfigValue("ScreenshotDirectory", "Screenshots");

    public static string GetFailureLogDirectory() =>
        GetConfigValue("FailureLogDirectory", "FailureLogs");

    public static bool WriteFailureLogOnFailure() =>
        bool.TryParse(GetConfigValue("WriteFailureLogOnFailure", "true"), out var result) && result;

    public static bool TakeScreenshotOnFailure() =>
        bool.TryParse(GetConfigValue("TakeScreenshotOnFailure", "true"), out var result) && result;

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

        var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
        return ConfigurationManager
            .OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None)
            .AppSettings.Settings;
    }
}
