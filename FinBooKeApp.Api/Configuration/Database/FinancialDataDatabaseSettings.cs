namespace FinBooKeApp.Api.Configuration.Database;

public class FinanceDatabaseSettings
{
    public const string SectionName = "FinancialDataDatabase";

    public string ConnectionString { get; set; } = "";

    public string DatabaseName { get; set; } = "";
}
