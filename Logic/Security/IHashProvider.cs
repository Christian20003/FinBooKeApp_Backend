namespace FinBooKeAPI.Logic.Security;

public interface IHashProvider
{
    /// <summary>
    /// Hashes the provided value.
    /// </summary>
    /// <param name="value">The value that should be hashed</param>
    /// <returns>The generated hash</returns>/
    public string Hash(string value);
}
