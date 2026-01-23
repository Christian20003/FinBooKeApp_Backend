using FinBookeAPI.Collections.TokenCollection;
using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Services.Token;

public partial class TokenService(
    ITokenCollection collection,
    IOptions<JwtSettings> settings,
    ILogger<TokenService> logger
) : ITokenService
{
    private readonly ITokenCollection _collection = collection;
    private readonly IOptions<JwtSettings> _settings = settings;
    private readonly ILogger<TokenService> _logger = logger;

    [LoggerMessage(
        EventId = LogEvents.TokenInvalidIssuer,
        Level = LogLevel.Critical,
        Message = "Token: Invalid issuer - {issuer}"
    )]
    private partial void LogInvalidIssuer(string? issuer);

    [LoggerMessage(
        EventId = LogEvents.TokenInvalidAudience,
        Level = LogLevel.Critical,
        Message = "Token: Invalid audience - {audience}"
    )]
    private partial void LogInvalidAudience(string? audience);

    [LoggerMessage(
        EventId = LogEvents.TokenInvalidSecret,
        Level = LogLevel.Critical,
        Message = "Token: Secret is invalid"
    )]
    private partial void LogInvalidSecret();

    [LoggerMessage(
        EventId = LogEvents.TokenInvalidAccessTokenLifetime,
        Level = LogLevel.Critical,
        Message = "Token: Invalid access token lifetime - {lifetime}"
    )]
    private partial void LogInvalidAccessTokenLifetime(int lifetime);

    [LoggerMessage(
        EventId = LogEvents.TokenInvalidRefreshTokenLifetime,
        Level = LogLevel.Critical,
        Message = "Token: Invalid refresh token lifetime - {lifetime}"
    )]
    private partial void LogInvalidRefreshTokenLifetime(int lifetime);
}
