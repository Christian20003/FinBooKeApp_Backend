using FinBooKeApp.Core.Shared.Email.Interfaces;
using FinBooKeApp.Core.Shared.Email.Models;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Dependencies;

public static class MockEmailProvider
{
    public record InMemorySmtpServer
    {
        public List<EmailPayload> Mails { get; init; } = [];
    }

    public static Mock<IEmailProvider> GetMock(InMemorySmtpServer server)
    {
        var mock = new Mock<IEmailProvider>();
        mock.Setup(obj => obj.Send(It.IsAny<EmailPayload>()))
            .Callback(
                (EmailPayload payload) =>
                {
                    server.Mails.Add(payload);
                }
            );
        return mock;
    }
}
