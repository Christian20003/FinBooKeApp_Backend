using System.Security.Claims;
using FinBooKeAPI.Models.Logic.Authentication;
using FinBooKeAPI.Models.Settings;
using Microsoft.Extensions.Options;

namespace FinBooKeAPI.Mapping.Authentication;

public static class TokenMapper
{
    public static CreateTokenPayload GetAccessTokenCreateTokenPayload(
        IEnumerable<Claim> claims,
        DateTime Expiration,
        IOptions<AuthenticationSettings> authenticationSettings
    )
    {
        return new CreateTokenPayload
        {
            Issuer = authenticationSettings.Value.Issuer,
            Audience = authenticationSettings.Value.Audience,
            Secret = authenticationSettings.Value.AccessTokenSecret,
            Expires = Expiration,
            Claims = claims,
        };
    }

    public static CreateTokenPayload GetRefreshTokenCreatePayload(
        IEnumerable<Claim> claims,
        DateTime Expiration,
        IOptions<AuthenticationSettings> authenticationSettings
    )
    {
        return new CreateTokenPayload
        {
            Issuer = authenticationSettings.Value.Issuer,
            Audience = authenticationSettings.Value.Audience,
            Secret = authenticationSettings.Value.RefreshTokenSecret,
            Expires = Expiration,
            Claims = claims,
        };
    }
}
