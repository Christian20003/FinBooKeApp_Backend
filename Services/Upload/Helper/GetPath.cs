using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    /// <summary>
    /// This method returns the path where the new file
    /// should be stored.
    /// </summary>
    /// <param name="upload">
    /// The file upload.
    /// </param>
    /// <param name="userId">
    /// The id of the user who has access to the file.
    /// </param>
    /// <param name="type">
    /// The upload type.
    /// </param>
    /// <returns>
    /// The path where the file should be uploaded.
    /// </returns>
    private string GetPath(FileUpload upload, Guid userId, UploadType type)
    {
        LogGetPath(userId, type);
        var date = upload.CreatedAt;
        var extension = Path.GetExtension(upload.File.FileName).ToLowerInvariant();
        var id = Guid.NewGuid();
        var name = type switch
        {
            UploadType.BANK_STATEMENT => $"bank_statement_{date.Month}_{date.Year}{id}{extension}",
            UploadType.RECEIPT => $"receipt_{date.Month}_{date.Year}{id}{extension}",
            _ => $"profile_image_{id}{extension}",
        };
        return GetPath(userId, type, name);
    }

    /// <summary>
    /// This method returns the path where a file is stored.
    /// </summary>
    /// <param name="userId">
    /// The id of the user who has access to this file.
    /// </param>
    /// <param name="type">
    /// The upload type.
    /// </param>
    /// <param name="name">
    /// The name of the file.
    /// </param>
    /// <returns>
    /// The path of the file.
    /// </returns>
    private string GetPath(Guid userId, UploadType type, string name)
    {
        LogGetPath(userId, type, name);
        var path = Path.Combine(_options.Value.Root, userId.ToString());
        switch (type)
        {
            case UploadType.BANK_STATEMENT:
                path = Path.Combine(path, "statements");
                break;
            case UploadType.RECEIPT:
                path = Path.Combine(path, "receipts");
                break;
            case UploadType.IMAGE:
            default:
                break;
        }
        return Path.Combine(path, name);
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Upload: Get storage path of file - User: {UserId}, Type: {Type}, Name: {Name}"
    )]
    private partial void LogGetPath(Guid userId, UploadType type, string name);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Upload: Get storage path of new file - User: {userId}, Type: {Type}"
    )]
    private partial void LogGetPath(Guid userId, UploadType type);
}
