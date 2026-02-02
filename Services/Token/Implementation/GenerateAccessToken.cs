using System.Security.Claims;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Token;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public JwtToken GenerateAccessToken(string userId)
    {
        LogGenerateAccessToken(userId);
        var lifetime = _settings.Value.AccessTokenExpireM;
        var secret = _settings.Value.AccessTokenSecret;
        if (lifetime <= 0)
        {
            LogInvalidAccessTokenLifetime(lifetime);
            throw new ApplicationException(
                "Expiration time of access tokens must be larger than zero"
            );
        }
        if (secret == null)
        {
            LogInvalidSecret();
            throw new ApplicationException("AccessTokenSecret is null");
        }
        var expire = DateTime.UtcNow.AddMinutes(lifetime);
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var token = GenerateToken(claims, secret, expire);
        LogGenerateAccessTokenSuccess(userId, expire);
        return new JwtToken { Value = token, Expires = expire.Ticks };
    }

    [LoggerMessage(
        EventId = LogEvents.TokenCreateAccessToken,
        Level = LogLevel.Information,
        Message = "Token: Generate access token for user - {userId}"
    )]
    private partial void LogGenerateAccessToken(string userId);

    [LoggerMessage(
        EventId = LogEvents.TokenCreateAccessTokenSuccess,
        Level = LogLevel.Information,
        Message = "Token: Successfully generated access token for user - {userId} with expiration at {expire}"
    )]
    private partial void LogGenerateAccessTokenSuccess(string userId, DateTime expire);
}
