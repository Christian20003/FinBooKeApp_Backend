using FinBooKeAPI.Logic.Parsing;

namespace FinBooKeAPI.Services.DataImport;

public partial class DataImportService : IDataImportService
{
    private static readonly string MISSING_DATA_KEY = "MissingData";
    private static readonly string INVALID_CREDENTIALS_KEY = "InvalidCredentials";
    private static readonly string MISSING_FILE_KEY = "MissingFile";
    private static readonly string UNSUPPORTED_FILE_FORMAT_KEY = "UnsupportedFileFormat";
    private static readonly Dictionary<string, ParserType> SUPPORTED_FORMAT_TYPES = new()
    {
        { ".json", ParserType.JSON },
        { ".xml", ParserType.XML },
        { ".csv", ParserType.CSV },
    };
}
