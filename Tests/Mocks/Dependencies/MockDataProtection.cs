using FinBooKeAPI.Logic.Security;
using Moq;

namespace FinBooKeAPI.Tests.Mocks.Dependencies;

public static class MockDataProtection
{
    public static Mock<IDataProtection> GetMock()
    {
        var mock = new Mock<IDataProtection>();
        mock.Setup(obj => obj.UnprotectEmail(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
        mock.Setup(obj => obj.Unprotect(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
        mock.Setup(obj => obj.ProtectEmail(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
        mock.Setup(obj => obj.Protect(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );

        return mock;
    }
}
