using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    /// <summary>
    /// This method creates a new file in the servers directory.
    /// File permissions will only be adjusted on linux based
    /// systems.
    /// </summary>
    /// <param name="upload">
    /// The file to be uploaded.
    /// </param>
    /// <param name="path">
    /// The path were the file should be stored.
    /// </param>
    /// <param name="name">
    /// The name of the file.
    /// </param>
    /// <returns>
    /// A unique name extended with a GUID.
    /// </returns>
    /// <exception cref="FormatException">
    /// If the file exceeds the size limit.
    /// If the file has not the correct format.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The file or directory cannot be created due to missing permissions.
    /// </exception>
    private async Task<string> CreateFile(FileUpload upload, string path, string name)
    {
        LogCreateFile(name, path);
        var file = upload.File;
        CheckPath(path);
        if (!HasValidSize(file))
        {
            LogInvalidFileSize(file.FileName);
            throw new FormatException(
                $"File can only have a size of {_options.Value.MaxFileSize} MB"
            );
        }
        if (!HasValidFormat(file))
        {
            LogInvalidFileFormat(file.FileName);
            throw new FormatException(
                $"File can only have one of the following formats: {string.Join(",", _options.Value.FileFormats.Values)}"
            );
        }
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueName = $"{name}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(path, uniqueName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        if (OperatingSystem.IsLinux())
            File.SetUnixFileMode(filePath, UnixFileMode.UserRead);
        else
            LogOSNotSupported();
        return uniqueName;
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Upload: Create file - {name}, {path}")]
    private partial void LogCreateFile(string name, string path);

    [LoggerMessage(
        EventId = LogEvents.UploadInvalidFileFormat,
        Level = LogLevel.Error,
        Message = "Upload: Invalid file format - {name}"
    )]
    private partial void LogInvalidFileFormat(string name);

    [LoggerMessage(
        EventId = LogEvents.UploadInvalidFileSize,
        Level = LogLevel.Error,
        Message = "Upload: Invalid file size - {name}"
    )]
    private partial void LogInvalidFileSize(string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Upload: OS not supported for setting file permissions"
    )]
    private partial void LogOSNotSupported();
}
