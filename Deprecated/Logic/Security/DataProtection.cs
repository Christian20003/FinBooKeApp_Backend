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

    public string ProtectEmail(string email)
    {
        var atSymbolIndex = email.IndexOf('@');
        var personalPart = atSymbolIndex > 0 ? email[..atSymbolIndex] : email;
        var domainPart = atSymbolIndex > 0 ? email[atSymbolIndex..] : "";
        return Protect(personalPart) + domainPart;
    }

    public string Unprotect(string value)
    {
        return Protector.Unprotect(value);
    }

    public string UnprotectEmail(string email)
    {
        var atSymbolIndex = email.IndexOf('@');
        var personalPart = atSymbolIndex > 0 ? email[..atSymbolIndex] : email;
        var domainPart = atSymbolIndex > 0 ? email[atSymbolIndex..] : "";
        return Unprotect(personalPart) + domainPart;
    }
}
