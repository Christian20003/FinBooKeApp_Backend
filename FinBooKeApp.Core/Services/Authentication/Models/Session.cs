namespace FinBooKeApp.Core.Services.Authentication.Models;

public record Session
{
    public required string AccessToken { init; get; }
    public required string RefreshToken { get; init; }
    public required long AccessTokenExpire { get; init; }
    public required long RefreshTokenExpire { get; init; }
}
