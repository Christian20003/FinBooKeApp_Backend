using FinBookeAPI.Models.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    public async Task Logout(string accessToken, string refreshToken)
    {
        LogLogout(accessToken, refreshToken);
        try
        {
            await _tokenService.StoreAccessToken(accessToken);
        }
        catch (SecurityTokenExpiredException)
        {
            LogExpiredToken(accessToken);
        }
        try
        {
            await _tokenService.StoreRefreshToken(refreshToken);
        }
        catch (SecurityTokenExpiredException)
        {
            LogExpiredToken(refreshToken);
        }
        LogSucceededLogout(accessToken, refreshToken);
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLogout,
        Level = LogLevel.Information,
        Message = "Authentication: Try to logout - {AccessToken}, {RefreshToken}"
    )]
    private partial void LogLogout(string accessToken, string refreshToken);

    [LoggerMessage(
        EventId = LogEvents.OperationIgnored,
        Level = LogLevel.Information,
        Message = "Authentication: Token has already expired - {Token}"
    )]
    private partial void LogExpiredToken(string token);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSucceededLogout,
        Level = LogLevel.Information,
        Message = "Authentication: Successful logout - {AccessToken}, {RefreshToken}"
    )]
    private partial void LogSucceededLogout(string accessToken, string refreshToken);
}
