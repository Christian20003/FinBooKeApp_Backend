using System.Security.Cryptography;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.SecurityUtility;

public partial class SecurityUtilityService : ISecurityUtilityService
{
    public string Hash(string content)
    {
        LogHashGeneration();
        byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(content, salt, _iterations, _algorithm, _hashSize);
        LogHashGenerationSuccess();
        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool IsHash(string content, string contentHash)
    {
        LogHashVerification();
        string[] parts = contentHash.Split("-");
        if (parts.Length != 2)
        {
            LogInvalidHash(contentHash);
            throw new ArgumentException("The hash value is not valid", nameof(contentHash));
        }

        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);

        byte[] toVerify = Rfc2898DeriveBytes.Pbkdf2(
            content,
            salt,
            _iterations,
            _algorithm,
            _hashSize
        );
        LogHashVerificationSuccess();
        return hash.SequenceEqual(toVerify);
    }

    [LoggerMessage(
        EventId = LogEvents.SecurityHashGeneration,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Generate hash value"
    )]
    private partial void LogHashGeneration();

    [LoggerMessage(
        EventId = LogEvents.SecurityHashGenerationSuccess,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Successfully generated hash value"
    )]
    private partial void LogHashGenerationSuccess();

    [LoggerMessage(
        EventId = LogEvents.SecurityHashVerification,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Verify hash value"
    )]
    private partial void LogHashVerification();

    [LoggerMessage(
        EventId = LogEvents.SecurityHashVerificationSuccess,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Successfully verified hash value"
    )]
    private partial void LogHashVerificationSuccess();

    [LoggerMessage(
        EventId = LogEvents.SecurityInvalidHash,
        Level = LogLevel.Error,
        Message = "SecurityUtility: Invalid hash value - {hash}"
    )]
    private partial void LogInvalidHash(string hash);
}
