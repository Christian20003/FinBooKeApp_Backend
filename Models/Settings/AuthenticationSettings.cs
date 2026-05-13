using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.Models.Settings;

public record AuthenticationSettings
{
    public const string SectionName = "Authentication";

    [StringLength(
        int.MaxValue,
        MinimumLength = 1,
        ErrorMessage = "Authentication settings: Empty issuer"
    )]
    public string Issuer { get; set; } = "";

    [StringLength(
        int.MaxValue,
        MinimumLength = 1,
        ErrorMessage = "Authentication settings: Empty audience"
    )]
    public string Audience { get; set; } = "";

    [StringLength(
        int.MaxValue,
        MinimumLength = 32,
        ErrorMessage = "Authentication settings: Access token secret too short"
    )]
    public string AccessTokenSecret { get; set; } = "";

    [StringLength(
        int.MaxValue,
        MinimumLength = 8,
        ErrorMessage = "Authentication settings: Refresh token secret too short"
    )]
    public string RefreshTokenSecret { get; set; } = "";
}
