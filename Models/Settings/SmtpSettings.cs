using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.Settings;

public record SmtpSettings
{
    public const string SectionName = "Smtp";

    [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "SMTP settings: Empty host")]
    public string Host { get; set; } = "";

    [Range(1, 65535, ErrorMessage = "SMTP settings: Invalid port")]
    public int Port { get; set; } = 587;

    [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "SMTP settings: Empty username")]
    public string Username { get; set; } = "";

    [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "SMTP settings: Empty password")]
    public string Password { get; set; } = "";

    [EmailAddress(ErrorMessage = "SMTP settings: Invalid email address")]
    public string Address { get; set; } = "";
}
