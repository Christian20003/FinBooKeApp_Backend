using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.Email;
using FinBooKeAPI.Logic.FileSystem;
using FinBooKeAPI.Logic.Security;
using FinBooKeAPI.Mapping.Email;
using FinBooKeAPI.Models.Database.Account;
using FinBookeAPI.Models.Result;
using FinBooKeAPI.Models.Settings;
using FinBooKeAPI.Services.Profile;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Services.Profile;

public partial class ProfileService(
    IAccountCollection accountCollection,
    IUploadSystem upload,
    IHashProvider hashProvider,
    IDataProtection protection,
    IEmailProvider emailProvider,
    IEmailTemplateBuilder emailBuilder,
    IStringLocalizer<ProfileService> localizer,
    IOptions<AccountSettings> accountSettings,
    IOptions<SmtpSettings> smtpSettings,
    ILogger<ProfileService> logger
) : IProfileService
{
    private static readonly string CHANGE_EMAIL_SUBJECT_KEY = "ChangeEmailSubject";

    private readonly IAccountCollection _accountCollection = accountCollection;
    private readonly IUploadSystem _upload = upload;
    private readonly IHashProvider _hashProvider = hashProvider;
    private readonly IDataProtection _protection = protection;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IEmailTemplateBuilder _emailBuilder = emailBuilder;
    private readonly IStringLocalizer<ProfileService> _localizer = localizer;
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
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public Task<Result<bool, ServiceResultCode>> DeleteProfileImageAsync(
        Guid userId,
        string filename
    )
    {
        throw new NotImplementedException();
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
        var subject = _localizer.GetString(CHANGE_EMAIL_SUBJECT_KEY);
        var payload = EmailMapper.GetEmailPayload(_smtpSettings, body, email, subject);
        _emailProvider.Send(payload);

        LogGetChangeEmailTokenSuccess(userId);
        return Result.Ok<bool, ServiceResultCode>(true);
    }

    public Task<Result<bool, ServiceResultCode>> GetVerifyEmailTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string, ServiceResultCode>> SetProfileImageAsync(
        Guid userId,
        IFormFile image
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, ServiceResultCode>> SetProfileLanguage(
        Guid userId,
        LanguageType languageType
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, ServiceResultCode>> SetProfileThemeAsync(
        Guid userId,
        ThemeType themeType
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, ServiceResultCode>> SetUsernameAsync(Guid userId, string newUsername)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, ServiceResultCode>> VerifyEmailAsync(Guid userId, string token)
    {
        throw new NotImplementedException();
    }
}
