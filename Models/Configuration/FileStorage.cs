namespace FinBookeAPI.Models.Configuration;

/// <summary>
/// This class represents the application configuration for
/// file storage.
/// </summary>
public class FileStorage
{
    /// <summary>
    /// The key of the section in the JSON file.
    /// </summary>
    public const string SectionName = "FileStorage";

    /// <summary>
    /// The name of the root folder.
    /// </summary>
    public string Root { get; set; } = "";

    /// <summary>
    /// The maximum size of a file in MB.
    /// </summary>
    public long MaxFileSize { get; set; }

    /// <summary>
    /// A dictionary of all valid file formats. The key
    /// represents the file extension and the value
    /// the actual content type.
    /// </summary>
    public Dictionary<string, string> FileFormats { get; set; } = [];
}
