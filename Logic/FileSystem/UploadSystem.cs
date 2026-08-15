using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FinBooKeAPI.Logic.FileSystem;

public class UploadFileSystem(IFileSystem fileSystem, IOptions<FileStorage> options) : IUploadSystem
{
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly IOptions<FileStorage> _options = options;

    public string UploadImage(Guid userId, IFormFile image)
    {
        if (!image.ContentType.Contains("image"))
            throw new FormatException("File format is not supported");
        return UploadFile(userId, image);
    }

    public byte[] GetImageContent(Guid userId, string fileName)
    {
        return ReadFileContent(userId, fileName);
    }

    private bool IsFileFormatSupported(IFormFile image)
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

    private string UploadFile(Guid userId, IFormFile file, string subDir = "")
    {
        if (!IsFileFormatSupported(file))
            throw new FormatException("File format is not supported");
        if (!IsFileSizeSupported(file))
            throw new ArgumentOutOfRangeException(nameof(file), "File size is too large");
        var fileName = CreateFileName(file);
        var path = GetFilePath(userId, subDir);
        var filePath = _fileSystem.CombinePath(path, fileName);
        _fileSystem.WriteAllBytes(file, filePath);
        _fileSystem.SetFilePermission(filePath, [FilePermission.READ]);
        return fileName;
    }

    private string CreateFileName(IFormFile file)
    {
        var fileId = Guid.NewGuid();
        var fileName = _fileSystem.GetFileName(file.FileName);
        var extension = _fileSystem.GetFileExtension(file.FileName);
        return $"{fileName}_{fileId}{extension}";
    }

    private string GetFilePath(Guid userId, string subDir)
    {
        var root = _options.Value.Root;
        var userDir = userId.ToString();
        var path = _fileSystem.CombinePath(root, userDir);
        if (subDir != string.Empty)
            path = _fileSystem.CombinePath(path, subDir);
        if (_fileSystem.FileExists(path))
            return path;
        _fileSystem.CreateDirectory(path);
        return path;
    }

    private byte[] ReadFileContent(Guid userId, string fileName, string subDir = "")
    {
        var path = GetFilePath(userId, subDir);
        var filePath = _fileSystem.CombinePath(path, fileName);
        return _fileSystem.ReadAllBytes(filePath);
    }
}
