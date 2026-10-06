using FinBooKeApp.Api.Configuration.Database;
using FinBooKeApp.Core.Services.Authentication.Models;
using FinBooKeApp.Core.Services.Profile.Models;
using FinBooKeApp.Core.Shared.Email.Models;
using FinBooKeApp.Core.Shared.FileSystem.Models;

namespace FinBooKeApp.Api.Configuration.Settings;

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

        services
            .AddOptions<AccountSettings>()
            .Bind(configuration.GetSection(AccountSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<DataImport>(configuration.GetSection(DataImport.SectionName));
        services.Configure<FileStorage>(configuration.GetSection(FileStorage.SectionName));

        return services;
    }
}
