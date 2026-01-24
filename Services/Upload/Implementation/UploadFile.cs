using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public async Task<string> UploadFile(FileUpload upload, Guid userId, UploadType type)
    {
        LogUploadFile(userId, type, upload.File.FileName);

        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("Invalid user id", nameof(userId));
        }

        var file = upload.File;

        if (!HasValidSize(file))
        {
            LogInvalidFileSize(userId, file.FileName);
            throw new FormatException(
                $"File can only have a size of {_options.Value.MaxFileSize} MB"
            );
        }
        if (!HasValidFormat(file))
        {
            LogInvalidFileFormat(userId, file.FileName);
            throw new FormatException(
                $"File can only have one of the following formats: {string.Join(",", _options.Value.FileFormats.Values)}"
            );
        }

        var path = GetPath(upload, userId, type);
        CheckPath(Path.GetDirectoryName(path)!);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        if (OperatingSystem.IsLinux())
            File.SetUnixFileMode(path, UnixFileMode.UserRead);
        else
            LogOSNotSupported();

        var fileName = Path.GetFileName(path);
        LogUploadFileSuccess(userId, type, fileName);
        return fileName;
    }

    [LoggerMessage(
        EventId = LogEvents.UploadFile,
        Level = LogLevel.Information,
        Message = "Upload: Create new file - User: {UserId}, Type: {Type}, Name: {Name}"
    )]
    private partial void LogUploadFile(Guid userId, UploadType type, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadFileSuccess,
        Level = LogLevel.Information,
        Message = "Upload: File upload succeeded - User: {UserId}, Type: {Type}, Name: {Name}"
    )]
    private partial void LogUploadFileSuccess(Guid userId, UploadType type, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadInvalidFileFormat,
        Level = LogLevel.Error,
        Message = "Upload: Invalid file format - User: {UserId}, Name: {Name}"
    )]
    private partial void LogInvalidFileFormat(Guid userId, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadInvalidFileSize,
        Level = LogLevel.Error,
        Message = "Upload: Invalid file size - User: {UserId}, Name: {Name}"
    )]
    private partial void LogInvalidFileSize(Guid userId, string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Upload: OS not supported for setting file permissions"
    )]
    private partial void LogOSNotSupported();
}
