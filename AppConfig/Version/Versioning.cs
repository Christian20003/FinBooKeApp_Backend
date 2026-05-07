using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.AppConfig.Version;

public static class Versioning
{
    public static IServiceCollection AddVersioningConfig(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        return services;        
    }
}