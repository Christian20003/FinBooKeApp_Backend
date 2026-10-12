namespace FinBooKeApp.Core.Shared.FileSystem.Models;

public class FileFormatException : Exception
{
    public FileFormatException()
        : base() { }

    public FileFormatException(string? msg)
        : base(msg) { }

    public FileFormatException(string? msg, Exception exception)
        : base(msg, exception) { }
}
