namespace FinBooKeAPI.Logic.Security;

public interface IDataProtection
{
    public string Protect(string value);
    public string Unprotect(string value);
}
