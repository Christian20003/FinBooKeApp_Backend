using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;
using FinBookeAPI.Services.Upload;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBookeAPI.Tests.Upload;

public partial class UploadServiceUnitTests
{
    private readonly UploadService _service;
    private readonly Mock<IFormFile> _file;
    private readonly FileUpload _upload;
    private readonly FileStorage _settings = new()
    {
        Root = "./uploads",
        MaxFileSize = 20,
        FileFormats = { { ".png", "image/png" }, { ".pdf", "application/pdf" } },
    };

    public UploadServiceUnitTests()
    {
        var logger = new Mock<ILogger<UploadService>>();
        var options = new Mock<IOptions<FileStorage>>();

        options.Setup(obj => obj.Value).Returns(() => _settings);

        _file = new Mock<IFormFile>();
        _upload = new FileUpload { File = _file.Object };
        _service = new UploadService(options.Object, logger.Object);
    }

    ~UploadServiceUnitTests()
    {
        if (Directory.Exists(_settings.Root))
            Directory.Delete(_settings.Root, true);
    }
}
