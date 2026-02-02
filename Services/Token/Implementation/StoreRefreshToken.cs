using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Token;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public async Task StoreRefreshToken(string token)
    {
        LogStoreRefreshToken(token);
        var (_, expire) = VerifyRefreshToken(token);
        var tokenObj = new JwtToken { Value = token, Expires = expire };
        await _collection.Add(tokenObj);
        LogStoreRefreshTokenSuccess(token);
    }

    [LoggerMessage(
        EventId = LogEvents.TokenStoreRefreshToken,
        Level = LogLevel.Information,
        Message = "Token: Store refresh token in database - {token}"
    )]
    private partial void LogStoreRefreshToken(string token);

    [LoggerMessage(
        EventId = LogEvents.TokenStoreRefreshTokenSuccess,
        Level = LogLevel.Information,
        Message = "Token: Successfully stored refresh token - {token}"
    )]
    private partial void LogStoreRefreshTokenSuccess(string token);
}
