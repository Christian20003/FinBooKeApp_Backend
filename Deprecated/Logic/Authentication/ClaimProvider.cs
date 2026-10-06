using System.Security.Claims;
using FinBooKeAPI.Models.Exceptions;

namespace FinBooKeAPI.Logic.Authentication;

public class ClaimProvider : IClaimProvider
{
    public IEnumerable<Claim> CreateClaims(string userId, string email)
    {
        return new List<Claim>
        {
            { new(ClaimTypes.NameIdentifier, userId) },
            { new(ClaimTypes.Email, email) },
        };
    }

    public string GetEmail(ClaimsPrincipal claims)
    {
        var emailClaim =
            claims.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
            ?? throw new ClaimException("Email claim not found");
        return emailClaim.Value;
    }

    public DateTime GetExpires(ClaimsPrincipal claims)
    {
        var expireClaim =
            claims.Claims.FirstOrDefault(claim => claim.Type == "exp")
            ?? throw new ClaimException("Expires claim not found");
        return DateTimeOffset.FromUnixTimeSeconds(long.Parse(expireClaim.Value)).UtcDateTime;
    }

    public string GetUserId(ClaimsPrincipal claims)
    {
        var userIdClaim =
            claims.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?? throw new ClaimException("User id claim not found");
        return userIdClaim.Value;
    }
}
