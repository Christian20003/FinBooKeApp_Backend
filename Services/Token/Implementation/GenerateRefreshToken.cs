using System.Security.Claims;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Token;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public JwtToken GenerateRefreshToken(string userId)
    {
        LogGenerateRefreshToken(userId);
        var lifetime = _settings.Value.RefreshTokenExpireD;
        var secret = _settings.Value.RefreshTokenSecret;
        if (lifetime <= 0)
        {
            LogInvalidRefreshTokenLifetime(lifetime);
            throw new ApplicationException(
                "Expiration time of refresh tokens must be larger than zero"
            );
        }
        if (secret == null)
        {
            LogInvalidSecret();
            throw new ApplicationException("RefreshTokenSecret is null");
        }
        var expires = DateTime.UtcNow.AddDays(lifetime);
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var token = GenerateToken(claims, secret, expires);
        LogGenerateRefreshTokenSuccess(userId, expires);
        return new JwtToken { Value = token, Expires = expires.Ticks };
    }

    [LoggerMessage(
        EventId = LogEvents.TokenCreateRefreshToken,
        Level = LogLevel.Information,
        Message = "Token: Generate new refresh token for - {userId}"
    )]
    private partial void LogGenerateRefreshToken(string userId);

    [LoggerMessage(
        EventId = LogEvents.TokenCreateRefreshTokenSuccess,
        Level = LogLevel.Information,
        Message = "Token: Successfully generated refresh token for - {userId} expires at {expires}"
    )]
    private partial void LogGenerateRefreshTokenSuccess(string userId, DateTime expires);
}
