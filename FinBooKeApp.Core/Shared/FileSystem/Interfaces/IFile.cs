namespace FinBooKeApp.Core.Shared.FileSystem.Interfaces;

public interface IFile
{
    public string FileName { get; }
    public string ContentType { get; }
    public long Length { get; }
    public Stream OpenReadStream();
}
