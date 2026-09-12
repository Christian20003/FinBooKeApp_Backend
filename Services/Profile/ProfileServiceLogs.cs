using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Profile;

public partial class ProfileService : IProfileService
{
    [LoggerMessage(
        EventId = LogEvents.ProfileChangeEmailToken,
        Level = LogLevel.Information,
        Message = "Profile: Generate change email token - {UserId}"
    )]
    private partial void LogGetChangeEmailToken(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileChangeEmailTokenSuccess,
        Level = LogLevel.Information,
        Message = "Profile: Generated change email token successfully - {UserId}"
    )]
    private partial void LogGetChangeEmailTokenSuccess(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileUserNotFound,
        Level = LogLevel.Error,
        Message = "Profile: User account could not be found - {UserId}"
    )]
    private partial void LogUserNotFound(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileUserUpdateFailed,
        Level = LogLevel.Error,
        Message = "Profile: User account could not be updated - {UserId}"
    )]
    private partial void LogUserUpdateFailed(Guid userId);
}
