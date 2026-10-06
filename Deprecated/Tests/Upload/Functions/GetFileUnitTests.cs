using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Tests.Upload;

public partial class UploadServiceUnitTests
{
    [Fact(Skip = "This test is disabled")]
    public async Task Should_FailGettingFile_WhenUserIdIsInvalid()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetFile(Guid.Empty, UploadType.IMAGE, "file.pdf")
        );
    }

    [Fact(Skip = "This test is disabled")]
    public async Task Should_FailGettingFile_WhenFileNameIsInvalid()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetFile(Guid.NewGuid(), UploadType.IMAGE, "")
        );
    }

    [Fact(Skip = "This test is disabled")]
    public async Task Should_FailGettingFile_WhenFileDoesNotExist()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _service.GetFile(Guid.NewGuid(), UploadType.IMAGE, "file.pdf")
        );
    }

    [Fact(Skip = "This test is disabled")]
    public async Task Should_ReturnContentOfFile()
    {
        byte[] bytes = [1, 2, 3, 4, 5];
        var userId = Guid.NewGuid();
        var path = Path.Combine(_settings.Root, userId.ToString());
        var file = Path.Combine(path, "file.pdf");
        Directory.CreateDirectory(path);
        var stream = File.Create(file);
        stream.Write(bytes);
        stream.Close();

        var result = await _service.GetFile(userId, UploadType.IMAGE, "file.pdf");

        Assert.Equal(bytes, result);
    }
}
