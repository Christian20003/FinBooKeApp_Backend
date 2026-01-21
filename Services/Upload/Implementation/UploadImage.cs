using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public async Task<string> UploadImage(FileUpload upload, Guid userId)
    {
        _logger.LogDebug("Upload image");
        if (userId == Guid.Empty)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.UploadFileFailed,
                new ArgumentException("User id is empty", nameof(userId))
            );

        var path = GetImagePath(userId);
        var name = "profile_image";
        name = await CreateFile(upload, path, name);

        _logger.LogInformation(LogEvents.UploadFileSuccess, "File uploaded {name}", name);
        return name;
    }

    /* public byte[] DownloadImage(string name, Guid userId)
    {
        _logger.LogDebug("Download image");
        var path = GetImagePath(userId);
        return File.ReadAllBytes($"{path}/{name}");
    } */
}
