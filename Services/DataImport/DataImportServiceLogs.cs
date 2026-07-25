using FinBookeAPI.Models.Configuration;

namespace FinBooKeAPI.Services.DataImport;

public partial class DataImportService : IDataImportService
{
    [LoggerMessage(
        EventId = LogEvents.DataImportUser,
        Level = LogLevel.Information,
        Message = "DataImport: Import new user - {Filename}, {Path}"
    )]
    private partial void LogImportUser(string filename, string path);

    [LoggerMessage(
        EventId = LogEvents.DataImportMissingData,
        Level = LogLevel.Information,
        Message = "DataImport: Missing data - {EntryNumber}"
    )]
    private partial void LogMissingData(int entryNumber);

    [LoggerMessage(
        EventId = LogEvents.DataImportInsertFailed,
        Level = LogLevel.Information,
        Message = "DataImport: Entry could not be inserted - {EntryNumber}"
    )]
    private partial void LogInsertFailed(int entryNumber);

    [LoggerMessage(
        EventId = LogEvents.DataImportUserSucess,
        Level = LogLevel.Information,
        Message = "DataImport: Successfully import new user - {Filename}, {Path}"
    )]
    private partial void LogImportUserSuccess(string filename, string path);
}
