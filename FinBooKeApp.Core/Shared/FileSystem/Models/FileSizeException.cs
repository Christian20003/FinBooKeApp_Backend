namespace FinBooKeApp.Core.Shared.FileSystem.Models;

public class FileSizeException : Exception
{
    public FileSizeException()
        : base() { }

    public FileSizeException(string? msg)
        : base(msg) { }

    public FileSizeException(string? msg, Exception exception)
        : base(msg, exception) { }
}
