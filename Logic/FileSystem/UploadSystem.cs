using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FinBooKeAPI.Logic.FileSystem;

public class UploadFileSystem(IFileSystem fileSystem, IOptions<FileStorage> options) : IUploadSystem
{
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly IOptions<FileStorage> _options = options;

    public string UploadImage(Guid userId, IFormFile image)
    {
        if (!IsImageFileFormatSupported(image))
            throw new FormatException("File format is not supported");
        if (!IsFileSizeSupported(image))
            throw new ArgumentOutOfRangeException(nameof(image), "File size is too large");
        var fileName = CreateFileName(image);
        var path = GetFilePath(userId);
        var filePath = _fileSystem.CombinePath(path, fileName);
        _fileSystem.WriteAllBytes(image, filePath);
        return fileName;
    }

    private bool IsImageFileFormatSupported(IFormFile image)
    {
        var extension = _fileSystem.GetFileExtension(image.FileName);
        var formats = _options.Value.FileFormats;
        if (formats.TryGetValue(extension, out string? fileFormat) && fileFormat is not null)
            return fileFormat == image.ContentType;
        return false;
    }

    private bool IsFileSizeSupported(IFormFile file)
    {
        var fileSize = file.Length;
        var maxFileSize = _options.Value.MaxFileSizeMb * (1024 ^ 2);
        return fileSize <= maxFileSize;
    }

    private string CreateFileName(IFormFile file)
    {
        var fileId = Guid.NewGuid();
        var fileName = _fileSystem.GetFileName(file.FileName);
        var extension = _fileSystem.GetFileExtension(file.FileName);
        return $"{fileName}_{fileId}{extension}";
    }

    private string GetFilePath(Guid userId)
    {
        var root = _options.Value.Root;
        var userDir = userId.ToString();
        var path = _fileSystem.CombinePath(root, userDir);
        if (_fileSystem.FileExists(path))
            return path;
        _fileSystem.CreateDirectory(path);
        return path;
    }
}
