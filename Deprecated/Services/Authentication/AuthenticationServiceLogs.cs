using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    [LoggerMessage(
        EventId = LogEvents.AuthenticationLogin,
        Level = LogLevel.Information,
        Message = "Authentication: Login user - {Email}"
    )]
    private partial void LogLogin(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRegister,
        Level = LogLevel.Information,
        Message = "Authentication: Register user - {Email}"
    )]
    private partial void LogRegister(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLogout,
        Level = LogLevel.Information,
        Message = "Authentication: Logout user - {Id}"
    )]
    private partial void LogLogout(string id);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSendResetPasswordToken,
        Level = LogLevel.Information,
        Message = "Authentication: Request reset password token - {Email}"
    )]
    private partial void LogSendResetPwdToken(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationResetPassword,
        Level = LogLevel.Information,
        Message = "Authentication: Reset password of account - {Email}"
    )]
    private partial void LogResetPassword(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRefreshAccessToken,
        Level = LogLevel.Information,
        Message = "Authentication: Refresh access token - {Id}"
    )]
    private partial void LogRefreshAccessToken(string id);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLoginSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successful login - {Email}"
    )]
    private partial void LogLoginSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRegisterSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successful registration - {Email}"
    )]
    private partial void LogRegisterSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLogoutSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successful logout - {Email}"
    )]
    private partial void LogLogoutSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationResetPasswordTokenSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successfully send reset password token - {Email}"
    )]
    private partial void LogResetPasswordTokenSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationResetPasswordSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successful reset password - {Email}"
    )]
    private partial void LogResetPasswordSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRefreshAccessTokenSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successful refresh access token - {Email}"
    )]
    private partial void LogRefreshAccessTokenSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidCredentials,
        Level = LogLevel.Warning,
        Message = "Authentication: Invalid credentials - {Email}"
    )]
    private partial void LogInvalidCredentials(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationMissingRefreshToken,
        Level = LogLevel.Warning,
        Message = "Authentication: Missing refresh token - {Email}"
    )]
    private partial void LogMissingRefreshToken(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidRefreshToken,
        Level = LogLevel.Warning,
        Message = "Authentication: Invalid refresh token - {Email}"
    )]
    private partial void LogInvalidRefreshToken(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLockedAccount,
        Level = LogLevel.Warning,
        Message = "Authentication: Account locked - {Email}"
    )]
    private partial void LogAccountLock(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidUserId,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid user id - {Id}"
    )]
    private partial void LogInvalidUserId(string id);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInternalError,
        Level = LogLevel.Critical,
        Message = "Authentication: Internal error - {Messages}"
    )]
    private partial void LogInternalError(List<string> messages);
}
