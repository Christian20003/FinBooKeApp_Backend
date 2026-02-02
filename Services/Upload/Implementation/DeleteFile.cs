using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public void DeleteFile(Guid userId, UploadType type, string name)
    {
        LogDeleteFile(userId, type, name);
        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("Invalid user id", nameof(userId));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            LogInvalidFileName(name);
            throw new ArgumentException("Invalid file name", nameof(name));
        }
        var path = GetPath(userId, type, name);
        if (!Path.Exists(path))
        {
            LogFileNotFound(path);
            throw new FileNotFoundException($"Requested file {name} does not exist");
        }
        File.Delete(path);
        LogDeleteFileSuccess(userId, type, name);
    }

    [LoggerMessage(
        EventId = LogEvents.UploadDeleteFile,
        Level = LogLevel.Information,
        Message = "Upload: Delete file - User: {UserId}, Type: {Type}, Name: {Name}"
    )]
    private partial void LogDeleteFile(Guid userId, UploadType type, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadDeleteFileSuccess,
        Level = LogLevel.Information,
        Message = "Upload: Deleted file successfully - User: {UserId}, Type: {Type}, Name: {Name}"
    )]
    private partial void LogDeleteFileSuccess(Guid userId, UploadType type, string name);
}
