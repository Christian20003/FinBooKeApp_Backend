namespace FinBooKeAPI.Models.DTO.Authentication;

public record SessionDTO
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required long AccessTokenExpiresAt { get; init; }
    public required long RefreshTokenExpiresAt { get; init; }
}
