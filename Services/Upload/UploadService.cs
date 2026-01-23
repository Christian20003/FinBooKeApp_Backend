using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService(IOptions<FileStorage> options, ILogger<UploadService> logger)
    : IUploadService
{
    private readonly IOptions<FileStorage> _options = options;
    private readonly ILogger<UploadService> _logger = logger;

    [LoggerMessage(
        EventId = LogEvents.UploadFile,
        Level = LogLevel.Information,
        Message = "Upload: Get file upload - {userId}, file name - {name}"
    )]
    private partial void LogUploadFile(Guid userId, string name);

    [LoggerMessage(
        EventId = LogEvents.UploadFileSuccess,
        Level = LogLevel.Information,
        Message = "Upload: File upload succeeded for user - {userId}, file name - {name}"
    )]
    private partial void LogUploadFileSuccess(Guid userId, string name);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidUserId,
        Level = LogLevel.Error,
        Message = "Upload: Invalid user id - {userId}."
    )]
    private partial void LogInvalidUserId(Guid userId);
}
