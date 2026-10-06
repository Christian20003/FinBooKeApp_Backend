using FinBooKeApp.Core.Shared.FileSystem.Interfaces;

namespace FinBooKeApp.Api.Shared.Utils;

public class FileUpload(IFormFile file) : IFile
{
    private readonly IFormFile _file = file;

    public string FileName => _file.FileName;

    public string ContentType => _file.ContentType;

    public long Length => _file.Length;

    public Stream OpenReadStream()
    {
        return _file.OpenReadStream();
    }
}
