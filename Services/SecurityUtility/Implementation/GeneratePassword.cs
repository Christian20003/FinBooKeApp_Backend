using System.Security.Cryptography;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.SecurityUtility;

public partial class SecurityUtilityService : ISecurityUtilityService
{
    public string GeneratePassword(int length)
    {
        LogGeneratePassword(length);
        if (length <= 0)
        {
            LogInvalidPasswordLength(length);
            throw new ArgumentException("length must be larger than zero", nameof(length));
        }
        var options = new List<string>
        {
            _lowerCaseLetters,
            _upperCaseLetters,
            _digits,
            _specialChars,
        };
        var password = GetRandomString(length, string.Join("", options));
        var positions = new HashSet<int>();

        if (length < 4)
            return new string(password);

        while (positions.Count < 4)
        {
            positions.Add(RandomNumberGenerator.GetInt32(length));
        }

        var index = 0;
        foreach (var position in positions)
        {
            var option = options[index];
            var value = option[RandomNumberGenerator.GetInt32(option.Length)];
            password[position] = value;
            index++;
        }
        LogGeneratePasswordSuccess();
        return new string(password);
    }

    [LoggerMessage(
        EventId = LogEvents.SecurityCreatePassword,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Generate password with length - {length}"
    )]
    private partial void LogGeneratePassword(int length);

    [LoggerMessage(
        EventId = LogEvents.SecurityInvalidLength,
        Level = LogLevel.Error,
        Message = "SecurityUtility: Invalid password length - {length}"
    )]
    private partial void LogInvalidPasswordLength(int length);

    [LoggerMessage(
        EventId = LogEvents.SecurityCreatePasswordSuccess,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Successfully generated password"
    )]
    private partial void LogGeneratePasswordSuccess();
}
