using FinBookeAPI.Models.Result;

namespace FinBooKeAPI.Services.DataImport;

public interface IDataImportService
{
    public Task<Result<bool>> ImportUser(string filename, string path);
}
