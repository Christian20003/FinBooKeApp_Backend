using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public (string, long) VerifyRefreshToken(string token)
    {
        LogVerifyRefreshToken();
        var secret = _settings.Value.RefreshTokenSecret;
        if (secret == null)
        {
            LogInvalidSecret();
            throw new ApplicationException("Refresh token secret is null");
        }
        var (id, expires) = VerifyToken(token, secret);
        LogVerifyRefreshTokenSuccess();
        return (id, expires);
    }

    [LoggerMessage(
        EventId = LogEvents.TokenVerifyRefreshToken,
        Level = LogLevel.Information,
        Message = "Token: Verify refresh token"
    )]
    private partial void LogVerifyRefreshToken();

    [LoggerMessage(
        EventId = LogEvents.TokenVerifyRefreshTokenSuccess,
        Level = LogLevel.Information,
        Message = "Token: Successfully verified refresh token"
    )]
    private partial void LogVerifyRefreshTokenSuccess();
}
