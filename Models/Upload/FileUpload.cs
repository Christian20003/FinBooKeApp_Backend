namespace FinBookeAPI.Models.Upload;

/// <summary>
/// This class represent a file upload from a form.
/// </summary>
public class FileUpload
{
    /// <summary>
    /// The actual file.
    /// </summary>
    public required IFormFile File { get; set; }

    /// <summary>
    /// The date where this file has been created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
