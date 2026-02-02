using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.Tests.Upload;

public partial class UploadServiceUnitTests
{
    [Fact]
    public void Should_FailDeletingFile_WhenUserIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(
            () => _service.DeleteFile(Guid.Empty, UploadType.IMAGE, "file.pdf")
        );
    }

    [Fact]
    public void Should_FailDeletingFile_WhenFileDoesNotExist()
    {
        Assert.Throws<FileNotFoundException>(
            () => _service.DeleteFile(Guid.NewGuid(), UploadType.IMAGE, "file.pdf")
        );
    }

    [Fact]
    public void Should_FailDeletingFile_WhenFileNameIsInvalid()
    {
        Assert.Throws<ArgumentException>(
            () => _service.DeleteFile(Guid.NewGuid(), UploadType.IMAGE, "")
        );
    }

    [Fact]
    public void Should_DeleteFile_WhenFileExists()
    {
        byte[] bytes = [1, 2, 3, 4, 5];
        var userId = Guid.NewGuid();
        var path = Path.Combine(_settings.Root, userId.ToString());
        var file = Path.Combine(path, "file.pdf");
        Directory.CreateDirectory(path);
        var stream = File.Create(file);
        stream.Write(bytes);
        stream.Close();

        _service.DeleteFile(userId, UploadType.IMAGE, "file.pdf");

        Assert.False(Path.Exists(file));
    }
}
