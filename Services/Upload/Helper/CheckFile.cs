namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    /// <summary>
    /// This method checks if the file has a valid file size.
    /// </summary>
    /// <param name="file">
    /// The file that has been uploaded.
    /// </param>
    /// <returns>
    /// <c>true</c> if the file size is valid, otherwise <c>false</c>.
    /// </returns>
    private bool HasValidSize(IFormFile file)
    {
        return file.Length <= (_options.Value.MaxFileSize * 1024 * 1024);
    }

    /// <summary>
    /// This method checks if the file has a valid format.
    /// </summary>
    /// <param name="file">
    /// The file that has been uploaded.
    /// </param>
    /// <returns>
    /// <c>true</c> if the file format is valid, otherwise <c>false</c>.
    /// </returns>
    private bool HasValidFormat(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (_options.Value.FileFormats.TryGetValue(extension, out string? contentType))
            return contentType == file.ContentType;
        return false;
    }
}
