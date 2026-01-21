using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public interface IUploadService
{
    /// <summary>
    /// This method allows to upload a profile image.
    /// </summary>
    /// <param name="upload">
    /// The image file that should be uploaded.
    /// </param>
    /// <param name="userId">
    /// The id of the user who has access to this image.
    /// </param>
    /// <returns>
    /// The name of the file stored on the server.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the user id is an empty GUID.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the provided image exceeded the maximum file size.
    /// If the provided image has not a supported format.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// If a directory or the image file could not be created
    /// on the system due to missing permissions.
    /// </exception>
    public Task<string> UploadImage(FileUpload upload, Guid userId);
}
