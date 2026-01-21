namespace FinBookeAPI.Tests.Upload;

public partial class UploadServiceUnitTests
{
    [Fact]
    public async Task Should_FailUploadingImage_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UploadImage(_upload, Guid.Empty)
        );
    }

    [Fact]
    public async Task Should_FailUploadingImage_WhenFileSizeExceeded()
    {
        _file.Setup(obj => obj.Length).Returns(() => (_settings.MaxFileSize + 1) * 1024 * 1024);

        await Assert.ThrowsAsync<FormatException>(
            () => _service.UploadImage(_upload, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_FailUploadingImage_WhenFileFormatIsInvalid()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.txt");

        await Assert.ThrowsAsync<FormatException>(
            () => _service.UploadImage(_upload, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_StoreImageInUserSpecificDirectory()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.png");
        _file.Setup(obj => obj.ContentType).Returns(() => "image/png");

        var userId = Guid.NewGuid();
        var result = await _service.UploadImage(_upload, userId);
        var path = Path.Combine(_settings.Root, userId.ToString(), result);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Should_SetImageAccessControlToReadOnly()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.png");
        _file.Setup(obj => obj.ContentType).Returns(() => "image/png");

        var userId = Guid.NewGuid();
        var result = await _service.UploadImage(_upload, userId);
        var path = Path.Combine(_settings.Root, userId.ToString(), result);

        if (OperatingSystem.IsLinux())
        {
            var permissions = File.GetUnixFileMode(path);
            Assert.Equal(UnixFileMode.UserRead, permissions);
        }
    }
}
