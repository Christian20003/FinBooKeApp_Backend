namespace FinBookeAPI.Models.Configuration;

/// <summary>
/// Class <c>AuthDatabaseSettings</c> models the configurations of the authentication database from <c>appsettings.json</c>
/// and any specified secret.
/// </summary>
public class AuthDatabaseSettings
{
    /// <summary>
    /// The name of the section in the <c>appsettings.json</c> file.
    /// </summary>
    public const string SectionName = "AuthDatabase";

    /// <summary>
    /// The connection path to get access to the actual database instance.
    /// </summary>
    public string ConnectionString { get; set; } = "";

    /// <summary>
    /// The name of the database where all authentication data is stored.
    /// </summary>
    public string DatabaseName { get; set; } = "";
}
