using FinBooKeAPI.Models.Database.Account;
using FinBookeAPI.Models.Result;
using FinBooKeAPI.Services.Profile;

namespace FinBookeAPI.Services.Profile;

public interface IProfileService
{
    public Task<Result<bool, ServiceResultCode>> GetChangeEmailTokenAsync(
        Guid userId,
        string newEmail
    );

    public Task<Result<bool, ServiceResultCode>> ChangeEmailAsync(
        Guid userId,
        string token,
        string email
    );

    public Task<Result<bool, ServiceResultCode>> GetVerifyEmailTokenAsync(Guid userId);

    public Task<Result<bool, ServiceResultCode>> VerifyEmailAsync(Guid userId, string token);

    public Task<Result<bool, ServiceResultCode>> SetUsernameAsync(Guid userId, string newUsername);

    public Task<Result<string, ServiceResultCode>> SetProfileImageAsync(
        Guid userId,
        IFormFile image
    );

    public Task<Result<bool, ServiceResultCode>> DeleteProfileImageAsync(
        Guid userId,
        string filename
    );

    public Task<Result<bool, ServiceResultCode>> SetProfileThemeAsync(
        Guid userId,
        ThemeType themeType
    );

    public Task<Result<bool, ServiceResultCode>> SetProfileLanguageAsync(
        Guid userId,
        LanguageType languageType
    );
}
