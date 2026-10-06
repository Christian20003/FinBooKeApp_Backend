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
        EventId = LogEvents.DataImportMissingFile,
        Level = LogLevel.Information,
        Message = "DataImport: Missing file - {Filename}"
    )]
    private partial void LogMissingFile(string filename);

    [LoggerMessage(
        EventId = LogEvents.DataImportUnsupportedFormat,
        Level = LogLevel.Information,
        Message = "DataImport: Unsupported file type - {Filename}"
    )]
    private partial void LogUnsupportedFormat(string filename);

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
