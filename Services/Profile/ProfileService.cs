using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.FileSystem;
using FinBooKeAPI.Logic.Security;
using FinBooKeAPI.Models.Database.Account;
using FinBookeAPI.Models.Result;

namespace FinBookeAPI.Services.Profile;

public partial class ProfileService(
    IAccountCollection accountCollection,
    IUploadSystem upload,
    IHashProvider hashProvider,
    IDataProtection protection,
    ILogger<ProfileService> logger
) : IProfileService
{
    private readonly IAccountCollection _accountCollection = accountCollection;
    private readonly IUploadSystem _upload = upload;
    private readonly IHashProvider _hashProvider = hashProvider;
    private readonly IDataProtection _protection = protection;
    private readonly ILogger<ProfileService> _logger = logger;

    public Task<Result<bool>> ChangeEmailAsync(Guid userId, string token)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteProfileImageAsync(Guid userId, string filename)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> GetChangeEmailTokenAsync(Guid userId, string newEmail)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> GetVerifyEmailTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> SetProfileImageAsync(Guid userId, IFormFile image)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> SetProfileLanguage(Guid userId, LanguageType languageType)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> SetProfileThemeAsync(Guid userId, ThemeType themeType)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> SetUsernameAsync(Guid userId, string newUsername)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> VerifyEmailAsync(Guid userId, string token)
    {
        throw new NotImplementedException();
    }
}
