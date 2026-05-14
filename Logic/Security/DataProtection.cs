using Microsoft.AspNetCore.DataProtection;

namespace FinBooKeAPI.Logic.Security;

public class DataProtection(IDataProtectionProvider provider) : IDataProtection
{
    private const string PURPOSE = "protection";

    public IDataProtector Protector { get; set; } = provider.CreateProtector(PURPOSE);

    public string Protect(string value)
    {
        return Protector.Protect(value);
    }

    public string Unprotect(string value)
    {
        return Protector.Unprotect(value);
    }
}
