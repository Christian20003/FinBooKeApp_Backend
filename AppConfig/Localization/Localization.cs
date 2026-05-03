namespace FinBookeAPI.AppConfig.Localization;

public static class Localization
{
    private static readonly string RESOURCE_PATH = "Resources";
    private static readonly string[] SUPPORTED_CULTURES = ["en-US", "de-DE"];
    private static readonly string DEFAULT_CULTURE = "en-US";

    public static IServiceCollection AddLocalizationConfig(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = RESOURCE_PATH);
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SetDefaultCulture(DEFAULT_CULTURE);
            options.AddSupportedCultures(SUPPORTED_CULTURES);
            options.AddSupportedUICultures(SUPPORTED_CULTURES);
        });
        return services;
    }
}
