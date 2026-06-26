using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.Settings;

public record AuthenticationSettings
{
    public const string SectionName = "Authentication";

    [StringLength(
        int.MaxValue,
        MinimumLength = 1,
        ErrorMessage = "Authentication settings: Empty issuer"
    )]
    public string Issuer { get; init; } = string.Empty;

    [StringLength(
        int.MaxValue,
        MinimumLength = 1,
        ErrorMessage = "Authentication settings: Empty audience"
    )]
    public string Audience { get; init; } = string.Empty;

    [StringLength(
        int.MaxValue,
        MinimumLength = 32,
        ErrorMessage = "Authentication settings: Access token secret too short"
    )]
    public string AccessTokenSecret { get; init; } = string.Empty;

    [StringLength(
        int.MaxValue,
        MinimumLength = 8,
        ErrorMessage = "Authentication settings: Refresh token secret too short"
    )]
    public string RefreshTokenSecret { get; init; } = string.Empty;

    [Url(ErrorMessage = "Authentication settings: Reset password link must be a valid URL")]
    public string ResetPasswordLink { get; init; } = string.Empty;
}
