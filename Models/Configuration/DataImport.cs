namespace FinBookeAPI.Models.Configuration;

public record DataImport
{
    public const string SectionName = "DataImport";
    public string Path { get; set; } = "";
    public bool Import { get; set; } = false;
}
