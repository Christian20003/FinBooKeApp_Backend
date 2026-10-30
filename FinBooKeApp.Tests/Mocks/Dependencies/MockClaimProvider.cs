using System.Security.Claims;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Dependencies;

public static class MockClaimProvider
{
    public static Mock<IClaimProvider> GetMock()
    {
        var mock = new Mock<IClaimProvider>();
        mock.Setup(obj => obj.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(Guid.NewGuid().ToString());
        return mock;
    }
}
