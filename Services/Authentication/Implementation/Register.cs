using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    public async Task<User> Register(string email, string userName, string password)
    {
        LogRegister();

        if (userName == string.Empty)
        {
            LogInvalidUsername(userName);
            throw new ArgumentException("User name is empty", nameof(userName));
        }
        if (!VerifyEmail(email))
        {
            LogInvalidEmail(email);
            throw new ArgumentException($"{email} is not a valid email-address", nameof(email));
        }
        var newUser = new UserAccount
        {
            UserName = _protector.Protect(userName),
            Email = _protector.ProtectEmail(email),
        };
        var result = await _accountManager.CreateUserAsync(newUser, password);
        if (!result.Succeeded)
        {
            LogInvalidSettings();
            throw new IdentityResultException(result.Errors, "User account conditions violated");
        }

        newUser = await VerifyUserAccount(email);
        var refreshToken = _tokenService.GenerateRefreshToken(newUser.Id);
        var jwtToken = _tokenService.GenerateAccessToken(newUser.Id);

        LogSucceededRegister(newUser.Id);
        return new User
        {
            Id = Guid.Parse(newUser.Id),
            Name = _protector.Unprotect(newUser.UserName!),
            Email = _protector.UnprotectEmail(newUser.Email!),
            ImagePath = newUser.ImagePath,
            AccessToken = jwtToken,
            RefreshToken = refreshToken,
        };
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRegister,
        Level = LogLevel.Information,
        Message = "Authentication: Try to register new user"
    )]
    private partial void LogRegister();

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidUsername,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid username - {Username}"
    )]
    private partial void LogInvalidUsername(string userName);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidCredentials,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid registration data"
    )]
    private partial void LogInvalidSettings();

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSucceededRegister,
        Level = LogLevel.Information,
        Message = "Authentication: Successful registration - {UserId}"
    )]
    private partial void LogSucceededRegister(string userId);
}
