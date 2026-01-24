using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Tests.Upload;

public partial class UploadServiceUnitTests
{
    [Fact]
    public async Task Should_FailUploadingFile_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UploadFile(_upload, Guid.Empty, UploadType.IMAGE)
        );
    }

    [Fact]
    public async Task Should_FailUploadingFile_WhenFileSizeExceeded()
    {
        _file.Setup(obj => obj.Length).Returns(() => (_settings.MaxFileSize + 1) * 1024 * 1024);

        await Assert.ThrowsAsync<FormatException>(
            () => _service.UploadFile(_upload, Guid.NewGuid(), UploadType.IMAGE)
        );
    }

    [Fact]
    public async Task Should_FailUploadingFile_WhenFileFormatIsInvalid()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.txt");

        await Assert.ThrowsAsync<FormatException>(
            () => _service.UploadFile(_upload, Guid.NewGuid(), UploadType.IMAGE)
        );
    }

    [Fact]
    public async Task Should_StoreFileInUserSpecificDirectory()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.png");
        _file.Setup(obj => obj.ContentType).Returns(() => "image/png");

        var userId = Guid.NewGuid();
        var result = await _service.UploadFile(_upload, userId, UploadType.IMAGE);
        var path = Path.Combine(_settings.Root, userId.ToString(), result);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Should_SetFileAccessControlToReadOnly()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.png");
        _file.Setup(obj => obj.ContentType).Returns(() => "image/png");

        var userId = Guid.NewGuid();
        var result = await _service.UploadFile(_upload, userId, UploadType.IMAGE);
        var path = Path.Combine(_settings.Root, userId.ToString(), result);

        if (OperatingSystem.IsLinux())
        {
            var permissions = File.GetUnixFileMode(path);
            Assert.Equal(UnixFileMode.UserRead, permissions);
        }
    }
}
