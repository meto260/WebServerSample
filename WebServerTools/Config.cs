public static class Config {
    public static IConfigurationRoot AppSettings;
    public static string AppEnvironment;
    public static void Prepare() {
        AppSettings = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true).Build();
        AppEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    public static List<string> HttpPrefixes() {
        return AppSettings.GetSection("http_prefixes")
            .AsEnumerable()
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .Select(x => x.Value.ToString())
            .ToList();
    }

    public static Dictionary<string, string> MyResponseHeaders() {
        var result = AppSettings.GetSection("response_headers")
            .AsEnumerable()
            .Where(x => x.Value != null)
            .ToDictionary(
                x => x.Key.Split(':').Last(),
                y => y.Value
            );
        return result;
    }
}
