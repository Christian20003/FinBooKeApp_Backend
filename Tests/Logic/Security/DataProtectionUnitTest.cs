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

        _protectionMock
            .Setup(o => o.Protect(It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == input)))
            .Returns(protectedBytes);

        var actual = _protection.Protect(input);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Unprotect_Value()
    {
        var protectedInput = Base64UrlTextEncoder.Encode([1, 2, 3]);
        var expected = "expected";
        var unprotectedBytes = Encoding.UTF8.GetBytes(expected);

        _protectionMock
            .Setup(o =>
                o.Unprotect(
                    It.Is<byte[]>(b => b.SequenceEqual(Base64UrlTextEncoder.Decode(protectedInput)))
                )
            )
            .Returns(unprotectedBytes);

        var actual = _protection.Unprotect(protectedInput);

        Assert.Equal(expected, actual);
    }
}
