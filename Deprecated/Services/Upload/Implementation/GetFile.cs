using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public async Task<byte[]> GetFile(Guid userId, UploadType type, string name)
    {
        LogGetFile(userId, type, name);

        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("User id is invalid", nameof(userId));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            LogInvalidFileName(name);
            throw new ArgumentException("File name is invalid", nameof(name));
        }

        var path = GetPath(userId, type, name);

        if (!File.Exists(path))
        {
            LogFileNotFound(path);
            throw new FileNotFoundException($"Requested file {name} does not exist");
        }

        var content = await File.ReadAllBytesAsync(path);

        LogGetFileSuccess(userId, type, name);
        return content;
    }

    [LoggerMessage(
        EventId = LogEvents.UploadGetFile,
        Level = LogLevel.Information,
        Message = "Upload: Get content of image file - userId: {UserId}, Type: {Type}, name: {Name}"
    )]
    private partial void LogGetFile(Guid userId, UploadType type, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadGetFileSuccess,
        Level = LogLevel.Information,
        Message = "Upload: Get content of image file successfully - userId: {UserId}, Type: {Type}, name: {Name}"
    )]
    private partial void LogGetFileSuccess(Guid userId, UploadType type, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadInvalidFileName,
        Level = LogLevel.Error,
        Message = "Upload: Invalid file name - {Name}"
    )]
    private partial void LogInvalidFileName(string name);

    [LoggerMessage(
        EventId = LogEvents.UploadFileDoesNotExist,
        Level = LogLevel.Error,
        Message = "Upload: Requested file does not exist - {Path}"
    )]
    private partial void LogFileNotFound(string path);
}
