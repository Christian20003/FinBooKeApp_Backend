using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Dependencies;

public static class MockHttpContext
{
    public static Mock<HttpContext> GetMock()
    {
        var mock = new Mock<HttpContext>();
        mock.Setup(obj => obj.User).Returns(new ClaimsPrincipal());
        return mock;
    }
}
