using System.Security.Cryptography;

namespace FinBookeAPI.Services.SecurityUtility;

public partial class SecurityUtilityService(ILogger<SecurityUtilityService> logger)
    : ISecurityUtilityService
{
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA512;
    private readonly ILogger<SecurityUtilityService> _logger = logger;
    private const string _lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const string _upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string _digits = "0123456789";
    private const string _specialChars = "!§$%&/()=?{[]}";

    private const int _saltSize = 16;
    private const int _hashSize = 16;
    private const int _iterations = 100000;

    private const int _idGenerationTrials = 10;
}
