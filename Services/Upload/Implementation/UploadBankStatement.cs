using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public async Task<string> UploadBankStatement(FileUpload upload, Guid userId)
    {
        _logger.LogDebug("Upload bank statement");
        if (userId == Guid.Empty)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.UploadFileFailed,
                new ArgumentException("User id is empty", nameof(userId))
            );

        var path = GetBankStatementPath(userId);
        var name = $"bank_statement_{upload.CreatedAt.Month}_{upload.CreatedAt.Year}";
        name = await CreateFile(upload, path, name);

        _logger.LogInformation(LogEvents.UploadFileSuccess, "File uploaded {name}", name);
        return name;
    }
}
