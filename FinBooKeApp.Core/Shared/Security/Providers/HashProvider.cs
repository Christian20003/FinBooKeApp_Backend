using System.Security.Cryptography;
using System.Text;
using FinBooKeApp.Core.Shared.Security.Interfaces;

namespace FinBooKeApp.Core.Shared.Security.Providers;

public class HashProvider : IHashProvider
{
    public string Hash(string value)
    {
        byte[] hash = SHA512.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash);
    }
}
