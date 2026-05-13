namespace FinBooKeAPI.Models.Logic.Authentication;

public record VerifyTokenPayload
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
    public required string Token { get; set; }
}
