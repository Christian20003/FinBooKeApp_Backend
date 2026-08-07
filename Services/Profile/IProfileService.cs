using FinBooKeAPI.Models.Database.Account;
using FinBookeAPI.Models.Result;

namespace FinBookeAPI.Services.Profile;

public interface IProfileService
{
    public Task<Result<bool>> GetChangeEmailTokenAsync(Guid userId, string newEmail);

    public Task<Result<bool>> ChangeEmailAsync(Guid userId, string token);

    public Task<Result<bool>> GetVerifyEmailTokenAsync(Guid userId);

    public Task<Result<bool>> VerifyEmailAsync(Guid userId, string token);

    public Task<Result<bool>> SetUsernameAsync(Guid userId, string newUsername);

    public Task<Result<string>> SetProfileImageAsync(Guid userId, IFormFile image);

    public Task<Result<bool>> DeleteProfileImageAsync(Guid userId, string filename);

    public Task<Result<bool>> SetProfileThemeAsync(Guid userId, ThemeType themeType);

    public Task<Result<bool>> SetProfileLanguage(Guid userId, LanguageType languageType);
}
