using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public interface IUploadService
{
    /// <summary>
    /// This method allows to upload a file.
    /// </summary>
    /// <param name="upload">
    /// The file that should be uploaded.
    /// </param>
    /// <param name="userId">
    /// The id of the user who has access to this file.
    /// </param>
    /// <param name="type">
    /// The upload type.
    /// </param>
    /// <returns>
    /// The name of the file stored on the server.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the user id is an empty GUID.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the provided file exceeded the maximum file size.
    /// If the provided file has not a supported format.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// If a directory or the file could not be created
    /// on the system due to missing permissions.
    /// </exception>
    public Task<string> UploadFile(FileUpload upload, Guid userId, UploadType type);

    /// <summary>
    /// This method returns the content of the file with the
    /// provided name.
    /// </summary>
    /// <param name="userId">
    /// The id of the user who has access to the file.
    /// </param>
    /// <param name="type">
    /// The upload type.
    /// </param>
    /// <param name="name">
    /// The name of the file.
    /// </param>
    /// <returns>
    /// The content of the file as byte array.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the user id is an empty GUID.
    /// If the file name is an empty string.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// If the file does not exist.
    /// </exception>
    public Task<byte[]> GetFile(Guid userId, UploadType type, string name);
}
