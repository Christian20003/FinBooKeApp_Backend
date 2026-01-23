using System.Security.Cryptography;

namespace FinBookeAPI.Services.SecurityUtility;

public partial class SecurityUtilityService : ISecurityUtilityService
{
    /// <summary>
    /// This method generates a char array containing random characters.
    /// </summary>
    /// <param name="length">
    /// The length of the array.
    /// </param>
    /// <param name="options">
    /// All possible characters that can be used.
    /// </param>
    /// <returns>
    /// A char array containing random selected characters
    /// </returns>
    private char[] GetRandomString(int length, string options)
    {
        LogGetRandomString(length, options);
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            var index = RandomNumberGenerator.GetInt32(options.Length);
            result[i] = options[index];
        }
        return result;
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "SecurityUtility: Generate random string - length: {length}, options: {options}"
    )]
    private partial void LogGetRandomString(int length, string options);
}
