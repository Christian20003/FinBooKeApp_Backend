using FinBookeAPI.Models.Configuration;
using FinBooKeAPI.Models.Settings;

namespace FinBookeAPI.AppConfig.Settings;

public static class Settings
{
    public static IServiceCollection AddSettingsConfig(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AuthDatabaseSettings>(
            configuration.GetSection(AuthDatabaseSettings.SectionName)
        );
        services.Configure<FinanceDatabaseSettings>(
            configuration.GetSection(FinanceDatabaseSettings.SectionName)
        );
        services
            .AddOptions<AuthenticationSettings>()
            .Bind(configuration.GetSection(AuthenticationSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<SmtpSettings>()
            .Bind(configuration.GetSection(SmtpSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<DataImport>(configuration.GetSection(DataImport.SectionName));
        services.Configure<FileStorage>(configuration.GetSection(FileStorage.SectionName));

        return services;
    }
}
