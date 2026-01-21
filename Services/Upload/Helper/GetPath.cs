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
        return Path.Combine(_options.Value.Root, userId.ToString(), "receipts");
    }
}
