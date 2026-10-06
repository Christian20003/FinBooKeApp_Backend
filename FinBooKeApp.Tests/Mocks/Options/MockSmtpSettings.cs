using FinBooKeApp.Core.Shared.Email.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Options;

public static class MockSmtpSettings
{
    public static Mock<IOptions<SmtpSettings>> GetMock()
    {
        var settings = new SmtpSettings
        {
            Host = "host",
            Port = 1,
            Username = "username",
            Password = "password",
            Address = "host-email",
        };
        var mock = new Mock<IOptions<SmtpSettings>>();
        mock.Setup(obj => obj.Value).Returns(settings);
        return mock;
    }
}
