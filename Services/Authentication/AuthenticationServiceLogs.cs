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
        EventId = LogEvents.AuthenticationLoginSuccess,
        Level = LogLevel.Information,
        Message = "Authentication: Successful login - {Email}"
    )]
    private partial void LogLoginSuccess(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidCredentials,
        Level = LogLevel.Warning,
        Message = "Authentication: Invalid credentials - {Email}"
    )]
    private partial void LogInvalidCredentials(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLockedAccount,
        Level = LogLevel.Warning,
        Message = "Authentication: Account locked - {Email}"
    )]
    private partial void LogAccountLock(string email);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInternalError,
        Level = LogLevel.Critical,
        Message = "Authentication: Internal error - {Messages}"
    )]
    private partial void LogInternalError(List<string> messages);
}
