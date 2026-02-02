using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Token;

public partial class TokenService : ITokenService
{
    public async Task CleanTokenDatabase()
    {
        LogCleanTokenDatabase();
        await _collection.Delete();
    }

    [LoggerMessage(
        EventId = LogEvents.TokenCleanDatabase,
        Level = LogLevel.Information,
        Message = "Token: Clean token database"
    )]
    private partial void LogCleanTokenDatabase();
}
