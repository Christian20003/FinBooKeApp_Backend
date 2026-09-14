using FinBooKeAPI.Models.Settings;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBooKeAPI.Tests.Mocks.Dependencies;

public static class MockAccountSettings
{
    public static Mock<IOptions<AccountSettings>> GetMock()
    {
        var settings = new AccountSettings
        {
            ChangeEmailLink = "change/email/link",
            VerifyEmailLink = "verify/email/link",
        };
        var mock = new Mock<IOptions<AccountSettings>>();
        mock.Setup(obj => obj.Value).Returns(settings);
        return mock;
    }
}
