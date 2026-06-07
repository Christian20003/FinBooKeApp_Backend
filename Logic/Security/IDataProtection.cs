namespace FinBooKeAPI.Logic.Security;

public interface IDataProtection
{
    /// <summary>
    /// Protects the provided value cryptographically.
    /// </summary>
    /// <param name="value">The value that should be protected</param>
    /// <returns>The protected value</returns>
    public string Protect(string value);

    /// <summary>
    /// Derives the actual value cryptographically.
    /// </summary>
    /// <param name="value">The protected value</param>
    /// <returns>The unprotected value</returns>
    public string Unprotect(string value);

    /// <summary>
    /// Protects the provided email cryptographically.
    /// </summary>
    /// <param name="email">Email that should be proteced</param>
    /// <returns>The protected email</returns>
    public string ProtectEmail(string email);

    /// <summary>
    /// Derives the actual email cryptographically.
    /// </summary>
    /// <param name="email">The protected email</param>
    /// <returns>The unprotected email</returns>
    public string UnprotectEmail(string email);
}
