namespace FinBooKeApp.Api.Configuration.Database;

public class AuthDatabaseSettings
{
    public const string SectionName = "AuthDatabase";

    public string ConnectionString { get; set; } = "";

    public string DatabaseName { get; set; } = "";
}
