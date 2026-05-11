namespace FinBookeAPI.Models.Configuration;

/// <summary>
/// Class <c>FinanceDatabaseSettings</c> models the configurations of the financial data database from <c>appsettings.json</c>
/// and any specified secret.
/// </summary>
public class FinanceDatabaseSettings
{
    /// <summary>
    /// The name of the section in the <c>appsettings.json</c> file.
    /// </summary>
    public const string SectionName = "FinancialDataDatabase";

    /// <summary>
    /// The connection path to get access to the actual database instance.
    /// </summary>
    public string ConnectionString { get; set; } = "";

    /// <summary>
    /// The name of the database where all financial data is stored.
    /// </summary>
    public string DatabaseName { get; set; } = "";
}
