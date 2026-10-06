using System.Security.Cryptography;
using System.Text;

namespace FinBooKeAPI.Logic.Security;

public class HashProvider : IHashProvider
{
    public string Hash(string value)
    {
        byte[] hash = SHA512.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash);
    }
}
