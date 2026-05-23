using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    public async Task<User> Login(string email, string password)
    {
        throw new NotImplementedException();
        /* LogLogin();
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
        }; */
    }
}
