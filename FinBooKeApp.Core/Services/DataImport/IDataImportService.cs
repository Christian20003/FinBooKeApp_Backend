using FinBookeApp.Core.Shared.Result;

namespace FinBooKeApp.Core.Services.DataImport;

public interface IDataImportService
{
    public Task<Result<bool>> ImportUser(string filename, string path);
}
