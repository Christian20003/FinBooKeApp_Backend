namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    /// <summary>
    /// This method returns the path of the profile image.
    /// </summary>
    /// <param name="userId">
    /// The id of the user who has access on that image.
    /// </param>
    /// <returns>
    /// The path where this image is stored.
    /// </returns>
    private string GetImagePath(Guid userId)
    {
        LogGetImagePath(userId);
        return Path.Combine(_options.Value.Root, userId.ToString());
    }

    /// <summary>
    /// This method returns the path of all uploaded bank statement files.
    /// </summary>
    /// <param name="userId">
    /// The id of the user who has access on these statement files.
    /// </param>
    /// <returns>
    /// The path where all bank statement files are stored.
    /// </returns>
    private string GetBankStatementPath(Guid userId)
    {
        LogGetBankStatementPath(userId);
        return Path.Combine(_options.Value.Root, userId.ToString(), "statements");
    }

    /// <summary>
    /// This method returns the path of all uploaded receipt files.
    /// </summary>
    /// <param name="userId">
    /// The id if the user who has access on these receipt files.
    /// </param>
    /// <returns>
    /// The path where all receipt files are stored.
    /// </returns>
    private string GetReceiptPath(Guid userId)
    {
        LogGetReceiptPath(userId);
        return Path.Combine(_options.Value.Root, userId.ToString(), "receipts");
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Upload: Get image path for user - {userId}")]
    private partial void LogGetImagePath(Guid userId);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Upload: Get bank statement path for user - {userId}"
    )]
    private partial void LogGetBankStatementPath(Guid userId);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Upload: Get receipt path for user - {userId}"
    )]
    private partial void LogGetReceiptPath(Guid userId);
}
