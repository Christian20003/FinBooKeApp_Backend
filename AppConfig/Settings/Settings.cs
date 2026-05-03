using FinBookeAPI.Models.Configuration;

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
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<SmtpServer>(configuration.GetSection(SmtpServer.SectionName));
        services.Configure<DataImport>(configuration.GetSection(DataImport.SectionName));
        services.Configure<FileStorage>(configuration.GetSection(FileStorage.SectionName));

        return services;
    }
}
