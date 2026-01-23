using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    private const string TemplateFilePwd = "../../../Templates/Email/PasswordReset.html";

    public async Task ResetPassword(string email, string code)
    {
        LogResetPassword();
        if (!VerifyEmail(email))
        {
            LogInvalidEmail(email);
            throw new ArgumentException($"{email} is not a valid email address", nameof(email));
        }
        var user = await VerifyUserAccount(email);
        if (user.AccessCode == null || user.AccessCodeCreatedAt == null)
        {
            LogInvalidAccessCode(code);
            throw new AuthorizationException("Invalid access code");
        }
        var expire = user.AccessCodeCreatedAt.Value.AddMinutes(10);
        if (DateTime.UtcNow.Ticks - expire.Ticks > 0)
        {
            LogExpiredAccessCode(code);
            throw new AuthorizationException("Access code expired");
        }
        if (user.AccessCode != _protector.Protect(code))
        {
            LogInvalidAccessCode(code);
            throw new AuthorizationException("Invalid access code");
        }
        var password = _securityUtilityService.GeneratePassword(20);
        var subject = "Requested password reset";
        string? body;
        try
        {
            body = File.ReadAllText(TemplateFilePwd);
            body = body.Replace("{{password}}", password);
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
            throw new ApplicationException($"{TemplateFilePwd} could not be used", exception);
        }

        user.AccessCode = null;
        user.AccessCodeCreatedAt = null;
        await _accountManager.UpdateUserAsync(user);
        await _accountManager.SetPasswordAsync(user, password);

        _emailService.Send(email, subject, body);
        LogSucceededResetPassword();
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationResetPassword,
        Level = LogLevel.Information,
        Message = "Authentication: Try to reset password"
    )]
    private partial void LogResetPassword();

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidAccessCode,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid access code - {Code}"
    )]
    private partial void LogInvalidAccessCode(string code);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationExpiredAccessCode,
        Level = LogLevel.Error,
        Message = "Authentication: Expired access code - {Code}"
    )]
    private partial void LogExpiredAccessCode(string code);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationSucceededResetPassword,
        Level = LogLevel.Information,
        Message = "Authentication: Successful reset password"
    )]
    private partial void LogSucceededResetPassword();
}
