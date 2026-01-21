using FinBookeAPI.AppConfig.Documentation;
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
        _logger.LogDebug("Create a new file {name} in {path}", name, path);
        var file = upload.File;
        CheckPath(path);
        if (!HasValidSize(file))
            Logging.ThrowAndLogError(
                _logger,
                LogEvents.UploadFileFailed,
                new FormatException($"File can only have a size of {_options.Value.MaxFileSize} MB")
            );
        if (!HasValidFormat(file))
            Logging.ThrowAndLogError(
                _logger,
                LogEvents.UploadFileFailed,
                new FormatException(
                    $"File can only have one of the following formats: {string.Join(",", _options.Value.FileFormats.Values)}"
                )
            );

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueName = $"{name}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(path, uniqueName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        if (OperatingSystem.IsLinux())
            File.SetUnixFileMode(filePath, UnixFileMode.UserRead);
        else
            _logger.LogWarning(
                "File permissions could not be restricted. Running OS is not Linux."
            );
        return uniqueName;
    }
}
