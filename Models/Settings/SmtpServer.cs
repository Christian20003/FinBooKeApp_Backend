namespace FinBookeAPI.Models.Configuration;

/// <summary>
/// This class models the SMTP-Server settings to be able of sending mails.
/// </summary>
public class SmtpServer
{
    /// <summary>
    /// The name of the section in the <c>appsettings.json</c> file.
    /// </summary>
    public const string SectionName = "Smtp";

    /// <summary>
    /// The host of the SMTP-Server
    /// </summary>
    public string? Host { get; set; } = "";

    /// <summary>
    /// The port of the SMTP-Server
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// The username for authentication on the SMTP-Server
    /// </summary>
    public string? Username { get; set; } = "";

    /// <summary>
    /// The password for authentication on the SMTP-Server
    /// </summary>
    public string? Password { get; set; } = "";

    /// <summary>
    /// The email address that should be used for sending emails
    /// </summary>
    public string? Address { get; set; } = "";
}
