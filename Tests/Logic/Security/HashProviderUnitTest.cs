using FinBooKeAPI.Logic.Security;

namespace FinBooKeAPI.Tests.Logic.Security;

public class HashProviderUnitTest
{
    private readonly HashProvider _provider;

    public HashProviderUnitTest()
    {
        _provider = new HashProvider();
    }

    [Fact]
    public void Should_Hash_Value()
    {
        var hash = _provider.Hash("value");

        Assert.Matches(@"^[A-Fa-f0-9]{128}$", hash);
    }
}
