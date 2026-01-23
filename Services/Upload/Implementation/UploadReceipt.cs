using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    public async Task<string> UploadReceipt(FileUpload upload, Guid userId)
    {
        LogUploadFile(userId, upload.File.FileName);
        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("User id is empty", nameof(userId));
        }

        var path = GetReceiptPath(userId);
        var name = $"receipt_{upload.CreatedAt.Month}_{upload.CreatedAt.Year}";
        name = await CreateFile(upload, path, name);

        LogUploadFileSuccess(userId, name);
        return name;
    }
}
