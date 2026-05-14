using System.Security.Claims;

namespace FinBooKeAPI.Logic.Authentication;

public interface IClaimProvider
{
    public IEnumerable<Claim> CreateClaims(string userId, string email);
    public string GetUserId(ClaimsPrincipal claims);
    public string GetEmail(ClaimsPrincipal claims);
    public DateTime GetExpires(ClaimsPrincipal claims);
}
