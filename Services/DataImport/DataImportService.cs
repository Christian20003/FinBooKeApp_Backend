using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.FileSystem;
using FinBooKeAPI.Logic.Parsing;
using FinBooKeAPI.Logic.Security;
using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Models.Result;
using Microsoft.Extensions.Localization;

namespace FinBooKeAPI.Services.DataImport;

public partial class DataImportService(
    IAccountCollection accountCollection,
    IParserFactory fileParserFactory,
    IFileSystem fileSystem,
    IDataProtection protection,
    IHashProvider hashProvider,
    IStringLocalizer<DataImportService> stringLocalizer,
    ILogger<DataImportService> logger
) : IDataImportService
{
    private readonly IAccountCollection _accountCollection = accountCollection;
    private readonly IParserFactory _fileParserFactory = fileParserFactory;
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly IDataProtection _protection = protection;
    private readonly IHashProvider _hashProvider = hashProvider;
    private readonly IStringLocalizer<DataImportService> _stringLocalizer = stringLocalizer;
    private readonly ILogger<DataImportService> _logger = logger;

    public async Task<Result<bool>> ImportUser(string filename, string path)
    {
        LogImportUser(filename, path);
        var fullPath = _fileSystem.CombinePath(path, filename);
        if (!_fileSystem.FileExists(fullPath))
        {
            LogMissingFile(fullPath);
            return Result.NotFound<bool>(_stringLocalizer.GetString(MISSING_FILE_KEY));
        }
        var extension = _fileSystem.GetFileExtension(filename);
        if (!SUPPORTED_FORMAT_TYPES.TryGetValue(extension, out var parserType))
        {
            LogUnsupportedFormat(fullPath);
            return Result.BadRequest<bool>(_stringLocalizer.GetString(UNSUPPORTED_FILE_FORMAT_KEY));
        }
        var fileParser = _fileParserFactory.GetParser(parserType);
        var fileContent = _fileSystem.ReadAllText(fullPath);
        var users = fileParser.Parse<UserAccount>(fileContent);
        List<string> errors = [];
        for (int index = 0; index < users.Count(); index++)
        {
            var user = users.ElementAt(index);
            if (user.Email is null || user.PasswordHash is null || user.UserName is null)
            {
                LogMissingData(index);
                errors.Add($"{index}:{_stringLocalizer.GetString(MISSING_DATA_KEY)}");
                continue;
            }
            var email = user.Email!;
            user.Email = _protection.ProtectEmail(email);
            user.EmailHash = _hashProvider.Hash(email);
            var password = user.PasswordHash!;
            user.PasswordHash = "";

            var result = await _accountCollection.CreateAccountAsync(user, password);
            if (result.Succeeded)
                continue;
            LogInsertFailed(index);
            foreach (var error in result.Errors)
                errors.Add(
                    $"{index}:{_stringLocalizer.GetString(INVALID_CREDENTIALS_KEY)}-{error.Description}"
                );
        }
        if (errors.Count != 0)
            return Result.BadRequest<bool>(errors);
        LogImportUserSuccess(filename, path);
        return Result.Ok(true);
    }
}
