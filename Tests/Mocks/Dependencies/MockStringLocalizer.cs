using Microsoft.Extensions.Localization;
using Moq;

namespace FinBooKeAPI.Tests.Mocks.Dependencies;

public static class MockStringLocalizer
{
    public static Mock<IStringLocalizer<R>> GetMock<R>()
    {
        var mock = new Mock<IStringLocalizer<R>>();
        mock.Setup(obj => obj[It.IsAny<string>()]).Returns(new LocalizedString("key", "value"));
        return mock;
    }
}
