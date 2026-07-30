using FinBookeAPI.Models.Configuration;
using FinBooKeAPI.Services.DataImport;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.AppConfig.Database;

public static class ImportDataExtension
{
    public static async Task<IServiceCollection> ImportData(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<DataImport>>();
        var importService = provider.GetRequiredService<IDataImportService>();

        if (!options.Value.Import)
            return services;

        await importService.ImportUser("Users.json", options.Value.Path);

        return services;
    }
}
