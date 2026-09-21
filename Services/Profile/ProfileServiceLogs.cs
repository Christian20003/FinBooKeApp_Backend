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
        EventId = LogEvents.ProfileChangeEmail,
        Level = LogLevel.Information,
        Message = "Profile: Change email address - {UserId}"
    )]
    private partial void LogChangeEmail(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileDeleteImage,
        Level = LogLevel.Information,
        Message = "Profile: Delete profile image - {UserId}"
    )]
    private partial void LogDeleteProfileImage(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileChangeEmailTokenSuccess,
        Level = LogLevel.Information,
        Message = "Profile: Generated change email token successfully - {UserId}"
    )]
    private partial void LogGetChangeEmailTokenSuccess(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileChangeEmailSuccess,
        Level = LogLevel.Information,
        Message = "Profile: Changes email successfully - {UserId}"
    )]
    private partial void LogChangeEmailSuccess(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileDeleteProfileImageSuccess,
        Level = LogLevel.Information,
        Message = "Profile: Profile image deleted successfully - {UserId}"
    )]
    private partial void LogDeleteProfileImageSuccess(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileUserNotFound,
        Level = LogLevel.Error,
        Message = "Profile: User account could not be found - {UserId}"
    )]
    private partial void LogUserNotFound(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileEmailIdentical,
        Level = LogLevel.Warning,
        Message = "Profile: New email is identical to old one - {UserId}"
    )]
    private partial void LogEmailIdentical(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileEmailNotIdentical,
        Level = LogLevel.Error,
        Message = "Profile: previous email is different - {UserId}"
    )]
    private partial void LogEmailNotIdentical(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileInvalidToken,
        Level = LogLevel.Error,
        Message = "Profile: Token to verify email action is invalid - {UserId}"
    )]
    private partial void LogInvalidToken(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.ProfileUserUpdateFailed,
        Level = LogLevel.Error,
        Message = "Profile: User account could not be updated - {UserId}"
    )]
    private partial void LogUserUpdateFailed(Guid userId);
}
