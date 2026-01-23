using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public async Task<bool> TokenExists(string token)
    {
        LogTokenExists();
        return await _collection.Contains(token);
    }

    [LoggerMessage(
        EventId = LogEvents.TokenCheckExists,
        Level = LogLevel.Information,
        Message = "Token: Proof if token exists in database"
    )]
    private partial void LogTokenExists();
}
