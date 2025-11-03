using Microsoft.Extensions.Configuration;

namespace OnlineShoppingTests.Utils
{
    public static class ConfigManager
    {
        private static readonly IConfigurationRoot config;

        static ConfigManager()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            config = builder.Build();
        }

        private static string GetRequiredValue(string key)
        {
            var value = config[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Missing or empty configuration value for key: '{key}'");
            return value;
        }

        // UI Settings
        public static string Browser => GetRequiredValue("appSettings:browser");
        public static string GetUiBaseUrl() => GetRequiredValue("appSettings:baseUrl");
        public static int Timeout => int.TryParse(config["appSettings:timeout"], out var t) ? t : 10;
        public static bool Headless => bool.TryParse(config["appSettings:headless"], out var h) && h;
        public static string ScreenshotsFolder => GetRequiredValue("appSettings:screenshotsFolder");
        public static string TestReportsFolder => GetRequiredValue("appSettings:testReportsFolder");

        // API Settings
        public static string GetApiBaseUrl() => GetRequiredValue("apiSettings:baseUrl");
    }
}