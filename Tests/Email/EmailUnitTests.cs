using FinBooKeAPI.Models.Settings;
using FinBookeAPI.Services.Email;
using FinBookeAPI.Tests.Records;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBookeAPI.Tests.Email;

public partial class EmailUnitTests
{
    private readonly EmailService _service;
    private readonly SmtpSettings _options = SmtpSettingsRecord.GetObject();
    private const string _email = "alice@gmx.com";
    private const string _subject = "This is a test";
    private const string _body = "Hello World!";

    public EmailUnitTests()
    {
        var logger = new Mock<ILogger<EmailService>>();
        var settings = new Mock<IOptions<SmtpSettings>>();
        settings.Setup(obj => obj.Value).Returns(_options);
        _service = new EmailService(settings.Object, logger.Object);
    }
}
