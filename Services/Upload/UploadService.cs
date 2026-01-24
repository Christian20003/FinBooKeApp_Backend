using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Services.Upload;

public partial class UploadService(IOptions<FileStorage> options, ILogger<UploadService> logger)
    : IUploadService
{
    private readonly IOptions<FileStorage> _options = options;
    private readonly ILogger<UploadService> _logger = logger;

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidUserId,
        Level = LogLevel.Error,
        Message = "Upload: Invalid user id - {userId}."
    )]
    private partial void LogInvalidUserId(Guid userId);
}
