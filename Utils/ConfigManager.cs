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

        public static string Browser => GetRequiredValue("browser");
        public static string BaseUrl => GetRequiredValue("baseUrl");
        public static int Timeout => int.TryParse(config["Timeout"], out var t) ? t : 10;
        public static bool Headless => bool.TryParse(config["headless"], out var h) && h;
        public static string ScreenshotsFolder => GetRequiredValue("ScreenshotsFolder");
    }
}