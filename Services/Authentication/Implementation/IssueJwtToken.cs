using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Token;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    public async Task<JwtToken> IssueJwtToken(string refreshToken)
    {
        LogCreateToken();
        var (id, _) = _tokenService.VerifyRefreshToken(refreshToken);
        if (await _tokenService.TokenExists(refreshToken))
        {
            LogInvalidToken(refreshToken);
            throw new ArgumentException("Provided refresh token is revoked", nameof(refreshToken));
        }
        var result = _tokenService.GenerateAccessToken(id);
        LogCreatedToken();
        return result;
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationCreateToken,
        Level = LogLevel.Information,
        Message = "Authentication: Create new JWT access token"
    )]
    private partial void LogCreateToken();

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidToken,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid refresh token - {RefreshToken}"
    )]
    private partial void LogInvalidToken(string refreshToken);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationTokenCreated,
        Level = LogLevel.Information,
        Message = "Authentication: New JWT access token created"
    )]
    private partial void LogCreatedToken();
}
