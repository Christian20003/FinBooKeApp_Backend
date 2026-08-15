using FinBooKeAPI.Logic.FileSystem;
using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBooKeAPI.Tests.Logic.FileSystem;

public class UploadFileSystemUnitTest
{
    private readonly Mock<IFileSystem> _fileSystem;
    private readonly Mock<IOptions<FileStorage>> _options;
    private readonly UploadFileSystem _upload;

    public UploadFileSystemUnitTest()
    {
        _fileSystem = new Mock<IFileSystem>();
        _options = new Mock<IOptions<FileStorage>>();
        _upload = new UploadFileSystem(_fileSystem.Object, _options.Object);

        Setup();
    }

    private static FileStorage GetFileStorage()
    {
        return new FileStorage
        {
            Root = "./RootPath",
            MaxFileSizeMb = 5,
            FileFormats = new() { { ".jpg", "image/jpg" } },
        };
    }

    private static Mock<IFormFile> GetImageFile()
    {
        var image = new Mock<IFormFile>();
        var optionValues = GetFileStorage();
        image.Setup(obj => obj.FileName).Returns("image.jpg");
        image.Setup(obj => obj.ContentType).Returns("image/jpg");
        image.Setup(obj => obj.Length).Returns(optionValues.MaxFileSizeMb * 1024);
        return image;
    }

    private void Setup()
    {
        var optionValues = GetFileStorage();
        _options.Setup(obj => obj.Value).Returns(optionValues);

        _fileSystem
            .Setup(obj => obj.CombinePath(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(
                (string path1, string path2) =>
                {
                    return $"{path1}/{path2}";
                }
            );

        _fileSystem.Setup(obj => obj.FileExists(It.IsAny<string>())).Returns(true);

        _fileSystem
            .Setup(obj => obj.GetFileExtension(It.IsAny<string>()))
            .Returns(
                (string fileName) =>
                {
                    return Path.GetExtension(fileName);
                }
            );

        _fileSystem
            .Setup(obj => obj.GetFileName(It.IsAny<string>()))
            .Returns(
                (string fileName) =>
                {
                    return Path.GetFileNameWithoutExtension(fileName);
                }
            );
    }

    [Fact]
    public void UploadImage_WhenFileFormatNotSupported_ThrowException()
    {
        var image = GetImageFile();
        var userId = Guid.NewGuid();
        image.Setup(obj => obj.FileName).Returns("image.pdf");

        Assert.Throws<FormatException>(() => _upload.UploadImage(userId, image.Object));
    }

    [Fact]
    public void UploadImage_WhenFileSizeExceedsLimit_ThrowException()
    {
        var image = GetImageFile();
        var userId = Guid.NewGuid();
        var optionValues = GetFileStorage();
        image.Setup(obj => obj.Length).Returns(optionValues.MaxFileSizeMb * (1024 ^ 3));

        Assert.Throws<ArgumentOutOfRangeException>(() => _upload.UploadImage(userId, image.Object));
    }

    [Fact]
    public void UploadImage_WhenImageIsValid_StoreFileInUserSpecificDirectory()
    {
        var image = GetImageFile();
        var userId = Guid.NewGuid();
        var optionValues = GetFileStorage();
        _fileSystem.Setup(obj => obj.FileExists(It.IsAny<string>())).Returns(false);
        var path = $"{optionValues.Root}/{userId}";

        _upload.UploadImage(userId, image.Object);

        _fileSystem.Verify(obj => obj.CreateDirectory(path), Times.Once());
    }

    [Fact]
    public void UploadImage_WhenImageIsValid_CreateUniqueFileName()
    {
        var image = GetImageFile();
        var userId = Guid.NewGuid();

        var fileName = _upload.UploadImage(userId, image.Object);

        Assert.NotEmpty(fileName);
        Assert.NotEqual(fileName, image.Object.FileName);
    }

    [Fact]
    public void UploadImage_WhenImageIsValid_WriteContentToServerFile()
    {
        var image = GetImageFile();
        var userId = Guid.NewGuid();

        _upload.UploadImage(userId, image.Object);

        _fileSystem.Verify(
            obj => obj.WriteAllBytes(image.Object, It.IsAny<string>()),
            Times.Once()
        );
    }

    [Fact]
    public void UploadImage_WhenImageIsValid_SetFilePermissionsToReadOnly()
    {
        var image = GetImageFile();
        var userId = Guid.NewGuid();
        List<FilePermission> permission = [FilePermission.READ];

        _upload.UploadImage(userId, image.Object);

        _fileSystem.Verify(
            obj => obj.SetFilePermission(It.IsAny<string>(), permission),
            Times.Once()
        );
    }

    [Fact]
    public void GetImageContent_WhenImageExists_ReturnImageContent()
    {
        var userId = Guid.NewGuid();
        var fileName = "image.jpg";
        var optionValues = GetFileStorage();
        var path = $"{optionValues.Root}/{userId}/{fileName}";

        _upload.GetImageContent(userId, fileName);

        _fileSystem.Verify(obj => obj.ReadAllBytes(path), Times.Once());
    }
}
