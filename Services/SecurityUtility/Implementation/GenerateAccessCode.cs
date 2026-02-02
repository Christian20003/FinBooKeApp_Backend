using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.SecurityUtility;

public partial class SecurityUtilityService : ISecurityUtilityService
{
    public string GenerateAccessCode(int length)
    {
        LogGenerateAccessCode(length);
        if (length <= 0)
        {
            LogInvalidAccessCodeLength(length);
            throw new ArgumentException("length must be larger than zero", nameof(length));
        }
        var options = _upperCaseLetters + _digits;
        var code = GetRandomString(length, options);
        LogGenerateAccessCodeSuccess();
        return new string(code);
    }

    [LoggerMessage(
        EventId = LogEvents.SecurityCreateAccessCode,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Generate access code with length: {length}"
    )]
    private partial void LogGenerateAccessCode(int length);

    [LoggerMessage(
        EventId = LogEvents.SecurityInvalidLength,
        Level = LogLevel.Error,
        Message = "SecurityUtility: Invalid access code length - {length}"
    )]
    private partial void LogInvalidAccessCodeLength(int length);

    [LoggerMessage(
        EventId = LogEvents.SecurityCreateAccessCodeSuccess,
        Level = LogLevel.Information,
        Message = "SecurityUtility: Successfully generated access code"
    )]
    private partial void LogGenerateAccessCodeSuccess();
}
