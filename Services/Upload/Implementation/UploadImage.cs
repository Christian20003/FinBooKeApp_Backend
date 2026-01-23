using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public async Task<string> UploadImage(FileUpload upload, Guid userId)
    {
        LogUploadFile(userId, upload.File.FileName);
        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("User id is empty", nameof(userId));
        }

        var path = GetImagePath(userId);
        var name = "profile_image";
        name = await CreateFile(upload, path, name);

        LogUploadFileSuccess(userId, name);
        return name;
    }

    /* public byte[] DownloadImage(string name, Guid userId)
    {
        _logger.LogDebug("Download image");
        var path = GetImagePath(userId);
        return File.ReadAllBytes($"{path}/{name}");
    } */
}
