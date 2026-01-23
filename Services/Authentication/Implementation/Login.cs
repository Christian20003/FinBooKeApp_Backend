using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    public async Task<User> Login(string email, string password)
    {
        LogLogin();
        if (!VerifyEmail(email))
        {
            LogInvalidEmail(email);
            throw new ArgumentException($"{email} is not a valid email-address", nameof(email));
        }
        var user = await VerifyUserAccount(email);
        if (user.IsRevoked)
        {
            LogRevokedAccount(user.Id);
            throw new ResourceLockedException($"User account of {email} has been revoked");
        }
        await VerifyPassword(user, password);

        var accessToken = _tokenService.GenerateAccessToken(user.Id);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        LogSucceededLogin(user.Id);

        return new User
        {
            Id = Guid.Parse(user.Id),
            Name = _protector.Unprotect(user.UserName!),
            Email = _protector.UnprotectEmail(user.Email!),
            ImagePath = user.ImagePath,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLogin,
        Level = LogLevel.Information,
        Message = "Authentication: Try to login user"
    )]
    private partial void LogLogin();

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRevokedAccount,
        Level = LogLevel.Error,
        Message = "Authentication: Revoked account of - {UserId}"
    )]
    private partial void LogRevokedAccount(string userId);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSucceededLogin,
        Level = LogLevel.Information,
        Message = "Authentication: Successful login of user - {UserId}"
    )]
    private partial void LogSucceededLogin(string userId);
}
