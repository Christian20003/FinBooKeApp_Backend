using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Token;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public async Task StoreAccessToken(string token)
    {
        LogStoreAccessToken(token);
        var (_, expire) = VerifyAccessToken(token);
        var tokenObj = new JwtToken { Value = token, Expires = expire };
        await _collection.Add(tokenObj);
        LogStoreAccessTokenSuccess(token);
    }

    [LoggerMessage(
        EventId = LogEvents.TokenStoreAccessToken,
        Level = LogLevel.Information,
        Message = "Token: Store access token in database - {token}"
    )]
    private partial void LogStoreAccessToken(string token);

    [LoggerMessage(
        EventId = LogEvents.TokenStoreAccessTokenSuccess,
        Level = LogLevel.Information,
        Message = "Token: Successfully stored access token - {token}"
    )]
    private partial void LogStoreAccessTokenSuccess(string token);
}
