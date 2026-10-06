using System.Text;
using FinBooKeAPI.Logic.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Moq;

namespace FinBooKeAPI.Tests.Logic.Security;

public class DataProtectionUnitTest
{
    private readonly DataProtection _protection;
    private readonly Mock<IDataProtector> _protectionMock;
    private readonly Mock<IDataProtectionProvider> _providerMock;

    public DataProtectionUnitTest()
    {
        _protectionMock = new Mock<IDataProtector>();
        _providerMock = new Mock<IDataProtectionProvider>();
        _providerMock
            .Setup(o => o.CreateProtector(It.IsAny<string>()))
            .Returns(_protectionMock.Object);
        _protection = new DataProtection(_providerMock.Object);
    }

    [Fact]
    public void Should_Protect_Value()
    {
        var input = "input";
        var protectedBytes = new byte[] { 1, 2, 3 };
        var expected = Base64UrlTextEncoder.Encode(protectedBytes);

        _protectionMock.Setup(o => o.Protect(It.IsAny<byte[]>())).Returns(protectedBytes);

        var actual = _protection.Protect(input);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Unprotect_Value()
    {
        var protectedInput = Base64UrlTextEncoder.Encode([1, 2, 3]);
        var expected = "expected";
        var unprotectedBytes = Encoding.UTF8.GetBytes(expected);

        _protectionMock.Setup(o => o.Unprotect(It.IsAny<byte[]>())).Returns(unprotectedBytes);

        var actual = _protection.Unprotect(protectedInput);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Protect_Email_Value()
    {
        var email = "user@example.com";
        var protectedBytes = Encoding.UTF8.GetBytes("user");
        var expected = Base64UrlTextEncoder.Encode(protectedBytes) + "@example.com";

        _protectionMock.Setup(o => o.Protect(It.IsAny<byte[]>())).Returns(protectedBytes);

        var actual = _protection.ProtectEmail(email);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Unprotect_Email_Value()
    {
        var protectedBytes = Encoding.UTF8.GetBytes("user");
        var protectedEmail = Base64UrlTextEncoder.Encode(protectedBytes) + "@example.com";
        var expected = "user@example.com";
        var unprotectedBytes = Encoding.UTF8.GetBytes("user");

        _protectionMock.Setup(o => o.Unprotect(It.IsAny<byte[]>())).Returns(unprotectedBytes);

        var actual = _protection.UnprotectEmail(protectedEmail);

        Assert.Equal(expected, actual);
    }
}
