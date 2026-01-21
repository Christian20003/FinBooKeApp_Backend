namespace FinBookeAPI.Tests.Upload;

public partial class UploadServiceUnitTests
{
    [Fact]
    public async Task Should_FailUploadingReceipt_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UploadReceipt(_upload, Guid.Empty)
        );
    }

    [Fact]
    public async Task Should_FailUploadingReceipt_WhenFileSizeExceeded()
    {
        _file.Setup(obj => obj.Length).Returns(() => (_settings.MaxFileSize + 1) * 1024 * 1024);

        await Assert.ThrowsAsync<FormatException>(
            () => _service.UploadReceipt(_upload, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_FailUploadingReceipt_WhenFileFormatIsInvalid()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.txt");

        await Assert.ThrowsAsync<FormatException>(
            () => _service.UploadReceipt(_upload, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_StoreReceiptInUserSpecificDirectory()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.png");
        _file.Setup(obj => obj.ContentType).Returns(() => "image/png");

        var userId = Guid.NewGuid();
        var result = await _service.UploadReceipt(_upload, userId);
        var path = Path.Combine(_settings.Root, userId.ToString(), "receipts", result);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Should_SetReceiptAccessControlToReadOnly()
    {
        _file.Setup(obj => obj.Length).Returns(() => 5000);
        _file.Setup(obj => obj.FileName).Returns(() => "filename.png");
        _file.Setup(obj => obj.ContentType).Returns(() => "image/png");

        var userId = Guid.NewGuid();
        var result = await _service.UploadReceipt(_upload, userId);
        var path = Path.Combine(_settings.Root, userId.ToString(), "receipts", result);

        if (OperatingSystem.IsLinux())
        {
            var permissions = File.GetUnixFileMode(path);
            Assert.Equal(UnixFileMode.UserRead, permissions);
        }
    }
}
