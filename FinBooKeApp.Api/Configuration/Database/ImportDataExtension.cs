using FinBooKeApp.Core.Services.DataImport;
using Microsoft.Extensions.Options;

namespace FinBooKeApp.Api.Configuration.Database;

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
