using FinBooKeApp.Core.Shared.Security.Interfaces;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Dependencies;

public static class MockHashProvider
{
    public static Mock<IHashProvider> GetMock()
    {
        var mock = new Mock<IHashProvider>();
        mock.Setup(obj => obj.Hash(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
        return mock;
    }
}
