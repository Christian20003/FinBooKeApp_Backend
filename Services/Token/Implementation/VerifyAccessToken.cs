using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public (string, long) VerifyAccessToken(string token)
    {
        LogVerifyAccessToken();
        var secret = _settings.Value.AccessTokenSecret;
        if (secret == null)
        {
            LogInvalidSecret();
            throw new ApplicationException("Access token secret is null");
        }
        var (id, expires) = VerifyToken(token, secret);
        LogVerifyAccessTokenSuccess();
        return (id, expires);
    }

    [LoggerMessage(
        EventId = LogEvents.TokenVerifyAccessToken,
        Level = LogLevel.Information,
        Message = "Token: Verify access token"
    )]
    private partial void LogVerifyAccessToken();

    [LoggerMessage(
        EventId = LogEvents.TokenVerifyAccessTokenSuccess,
        Level = LogLevel.Information,
        Message = "Token: Successfully verified access token"
    )]
    private partial void LogVerifyAccessTokenSuccess();
}
