using System.Security.Claims;

namespace FinBooKeAPI.Models.Logic.Authentication;

public record CreateTokenPayload
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
    public required DateTime Expires { get; set; }
    public required IEnumerable<Claim> Claims { get; set; }
}
