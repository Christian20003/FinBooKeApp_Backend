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
    private static void CheckPath(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
