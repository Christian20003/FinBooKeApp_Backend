using FinBooKeAPI.Logic.FileSystem;
using Moq;

namespace FinBooKeAPI.Tests.Mocks.Dependencies;

public static class MockUploadSystem
{
    public record InMemoryFileSystem
    {
        public Dictionary<string, string> Files { get; init; } = [];
    }

    public static Mock<IFormFile> GetMockFile()
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(obj => obj.FileName).Returns("filename.test");
        mock.Setup(obj => obj.OpenReadStream())
            .Returns(() =>
            {
                byte[] data = [72, 101, 108, 108, 111];
                return new MemoryStream(data);
            });
        return mock;
    }

    public static Mock<IUploadSystem> GetMock(InMemoryFileSystem fileSystem)
    {
        var mock = new Mock<IUploadSystem>();
        mock.Setup(obj => obj.UploadImage(It.IsAny<Guid>(), It.IsAny<IFormFile>()))
            .Returns(
                (Guid userId, IFormFile file) =>
                {
                    using var reader = new StreamReader(file.OpenReadStream());
                    string fileContent = reader.ReadToEnd();
                    fileSystem.Files.Add(file.FileName, fileContent);
                    return file.FileName;
                }
            );

        mock.Setup(obj => obj.GetImageContent(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(
                (Guid userId, string filename) =>
                {
                    var content = fileSystem.Files.GetValueOrDefault(filename);
                    if (content is null)
                        return [];
                    return System.Text.Encoding.UTF8.GetBytes(content);
                }
            );

        mock.Setup(obj => obj.DeleteImage(It.IsAny<string>()))
            .Callback(
                (string filename) =>
                {
                    fileSystem.Files.Remove(filename);
                }
            );

        return mock;
    }
}
