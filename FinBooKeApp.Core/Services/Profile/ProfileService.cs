using FinBooKeApp.Core.Services.Profile.Models;
using FinBooKeApp.Core.Shared.Email.Interfaces;
using FinBooKeApp.Core.Shared.Email.Models;
using FinBooKeApp.Core.Shared.FileSystem.Interfaces;
using FinBookeApp.Core.Shared.Result;
using FinBooKeApp.Core.Shared.Security.Interfaces;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinBooKeApp.Core.Services.Profile;

public partial class ProfileService(
    IAccountCollection accountCollection,
    IUploadSystem upload,
    IHashProvider hashProvider,
    IDataProtection protection,
    IEmailProvider emailProvider,
    IEmailTemplateBuilder emailBuilder,
    IOptions<AccountSettings> accountSettings,
    IOptions<SmtpSettings> smtpSettings,
    ILogger<ProfileService> logger
) : IProfileService
{
    private readonly IAccountCollection _accountCollection = accountCollection;
    private readonly IUploadSystem _upload = upload;
    private readonly IHashProvider _hashProvider = hashProvider;
    private readonly IDataProtection _protection = protection;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IEmailTemplateBuilder _emailBuilder = emailBuilder;
    private readonly IOptions<AccountSettings> _accountSettings = accountSettings;
    private readonly IOptions<SmtpSettings> _smtpSettings = smtpSettings;
    private readonly ILogger<ProfileService> _logger = logger;

    public async Task<Result<bool, ServiceResultCode>> ChangeEmailAsync(
        Guid userId,
        string token,
        string email
    )
    {
        LogChangeEmail(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        var emailHash = _hashProvider.Hash(email);
        if (user.ChangeEmailHash != emailHash)
        {
            LogEmailNotIdentical(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.EMAIL_INVALID);
        }
        var result = await _accountCollection.ChangeEmailAddressAsync(user, token, email);
        if (!result.Succeeded)
        {
            LogInvalidToken(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.TOKEN_INVALID);
        }
        LogChangeEmailSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public async Task<Result<bool, ServiceResultCode>> DeleteProfileImageAsync(
        Guid userId,
        string filename
    )
    {
        LogDeleteProfileImage(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        _upload.DeleteImage(filename);
        user.ImagePath = string.Empty;
        var result = await _accountCollection.UpdateAccountAsync(user);
        if (!result.Succeeded)
        {
            LogUserUpdateFailed(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_UPDATE_FAILED);
        }
        LogDeleteProfileImageSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public async Task<Result<bool, ServiceResultCode>> GetChangeEmailTokenAsync(
        Guid userId,
        string newEmail
    )
    {
        LogGetChangeEmailToken(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        user.ChangeEmailHash = _hashProvider.Hash(newEmail);
        if (user.EmailHash == user.ChangeEmailHash)
        {
            LogEmailIdentical(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.EMAIL_IDENTICAL);
        }
        var identResult = await _accountCollection.UpdateAccountAsync(user);
        if (!identResult.Succeeded)
        {
            LogUserUpdateFailed(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_UPDATE_FAILED);
        }
        var token = await _accountCollection.GenerateChangeEmailToken(user, newEmail);
        var link = $"{_accountSettings.Value.ChangeEmailLink}/?token={token}&email={newEmail}";
        var body = _emailBuilder.GetChangeEmailTemplate(link);
        var email = _protection.UnprotectEmail(user.Email!);
        var subject = EmailText.ChangeEmailSubject;
        var payload = GetEmailPayload(body, email, subject);
        _emailProvider.Send(payload);

        LogGetChangeEmailTokenSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public async Task<Result<bool, ServiceResultCode>> GetVerifyEmailTokenAsync(Guid userId)
    {
        LogVerifyEmailToken(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        var token = await _accountCollection.GenerateEmailVerificationToken(user);
        var link = $"{_accountSettings.Value.VerifyEmailLink}/?token={token}";
        var body = _emailBuilder.GetVerifyEmailTemplate(link);
        var email = _protection.UnprotectEmail(user.Email!);
        var subject = EmailText.VerifyEmailSubject;
        var payload = GetEmailPayload(body, email, subject);
        _emailProvider.Send(payload);

        LogVerifyEmailTokenSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public async Task<Result<string, ServiceResultCode>> SetProfileImageAsync(
        Guid userId,
        IFile image
    )
    {
        LogSetProfileImage(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<string, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        user.ImagePath = _upload.UploadImage(userId, image);
        var result = await _accountCollection.UpdateAccountAsync(user);
        if (!result.Succeeded)
        {
            LogUserUpdateFailed(userId);
            return Result.Error<string, ServiceResultCode>(ServiceResultCode.USER_UPDATE_FAILED);
        }
        LogSetProfileImageSuccess(userId);
        return Result.Ok<string, ServiceResultCode>(user.ImagePath);
    }

    public async Task<Result<bool, ServiceResultCode>> SetProfileLanguageAsync(
        Guid userId,
        LanguageType languageType
    )
    {
        LogSetProfileLanguage(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        user.FrontendLanguage = languageType;
        var result = await _accountCollection.UpdateAccountAsync(user);
        if (!result.Succeeded)
        {
            LogUserUpdateFailed(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_UPDATE_FAILED);
        }
        LogSetProfileLanguageSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public async Task<Result<bool, ServiceResultCode>> SetProfileThemeAsync(
        Guid userId,
        ThemeType themeType
    )
    {
        LogSetProfileTheme(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        user.ThemeType = themeType;
        var result = await _accountCollection.UpdateAccountAsync(user);
        if (!result.Succeeded)
        {
            LogUserUpdateFailed(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_UPDATE_FAILED);
        }
        LogSetProfileThemeSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public async Task<Result<bool, ServiceResultCode>> SetUsernameAsync(
        Guid userId,
        string newUsername
    )
    {
        LogSetUsername(userId);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.Id == userId.ToString()
        );
        if (user is null)
        {
            LogUserNotFound(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_NOT_FOUND);
        }
        user.UserName = newUsername;
        var result = await _accountCollection.UpdateAccountAsync(user);
        if (!result.Succeeded)
        {
            LogUserUpdateFailed(userId);
            return Result.Error<bool, ServiceResultCode>(ServiceResultCode.USER_UPDATE_FAILED);
        }
        LogSetUsernameSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public Task<Result<bool, ServiceResultCode>> VerifyEmailAsync(Guid userId, string token)
    {
        throw new NotImplementedException();
    }

    private EmailPayload GetEmailPayload(string body, string email, string subject)
    {
        return new EmailPayload
        {
            Host = _smtpSettings.Value.Host,
            Port = _smtpSettings.Value.Port,
            Username = _smtpSettings.Value.Username,
            Password = _smtpSettings.Value.Password,
            From = _smtpSettings.Value.Address,
            To = [email],
            Subject = subject,
            Body = body,
            IsHtml = true,
        };
    }
}
