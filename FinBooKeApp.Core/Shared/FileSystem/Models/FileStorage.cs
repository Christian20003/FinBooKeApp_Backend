namespace FinBooKeApp.Core.Shared.FileSystem.Models;

public class FileStorage
{
    public const string SectionName = "FileStorage";
    public string Root { get; set; } = "";
    public long MaxFileSizeMb { get; set; }
    public Dictionary<string, string> FileFormats { get; set; } = [];
}
