namespace FinBookeAPI.Services.Upload;

public partial class UploadService : IUploadService
{
    /// <summary>
    /// This method checks if the path exist. If the path does not
    /// exist, it will be created.
    /// </summary>
    /// <param name="path">
    /// The path to be checked.
    /// </param>
    /// <exception cref="UnauthorizedAccessException">
    /// The directory cannot be created due to missing permissions.
    /// </exception>
    private void CheckPath(string path)
    {
        LogCheckPath(path);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Upload: Check path - {path}")]
    private partial void LogCheckPath(string path);
}
