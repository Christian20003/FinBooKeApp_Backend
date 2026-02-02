using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    private const string TemplateFileCode = "../../../Templates/Email/AccessCode.html";

    public async Task SendAccessCode(string email)
    {
        LogSendAccessCode();
        if (!VerifyEmail(email))
        {
            LogInvalidEmail(email);
            throw new ArgumentException($"{email} is not a valid email address", nameof(email));
        }
        var user = await VerifyUserAccount(email);
        var code = _securityUtilityService.GenerateAccessCode(6);
        var subject = "Requested security code";
        string? body;
        try
        {
            body = File.ReadAllText(TemplateFileCode);
            body = body.Replace("{{code}}", code);
        }
        catch (Exception exception)
            when (exception is DirectoryNotFoundException
                || exception is FileNotFoundException
                || exception is IOException
            )
        {
            LogConfigurationError(
                exception.GetType().ToString(),
                exception.Message,
                exception.StackTrace
            );
            throw new ApplicationException($"{TemplateFileCode} could not be used", exception);
        }

        user.AccessCode = _protector.Protect(code);
        user.AccessCodeCreatedAt = DateTime.UtcNow;
        await _accountManager.UpdateUserAsync(user);

        _emailService.Send(email, subject, body);
        LogSucceededSendAccessCode();
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSendAccessCode,
        Level = LogLevel.Information,
        Message = "Authentication: Try to send access code"
    )]
    private partial void LogSendAccessCode();

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSucceededSendAccessCode,
        Level = LogLevel.Information,
        Message = "Authentication: Successful send access code"
    )]
    private partial void LogSucceededSendAccessCode();
}
