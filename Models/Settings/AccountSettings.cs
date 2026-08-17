using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.Settings;

public record AccountSettings
{
    public const string SectionName = "Account";

    [Url(ErrorMessage = "Account settings: Change email link must be a valid URL")]
    public string ChangeEmailLink { get; init; } = string.Empty;

    [Url(ErrorMessage = "Account settings: Verify email link must be a valid URL")]
    public string VerifyEmailLink { get; init; } = string.Empty;
}
