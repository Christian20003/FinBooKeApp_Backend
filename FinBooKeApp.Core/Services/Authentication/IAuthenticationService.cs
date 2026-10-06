using FinBooKeApp.Core.Services.Authentication.Models;
using FinBookeApp.Core.Shared.Result;

namespace FinBooKeApp.Core.Services.Authentication;

public interface IAuthenticationService
{
    public Task<Result<User>> LoginAsync(string email, string password);
    public Task<Result<User>> RegisterAsync(string email, string username, string password);
    public Task<Result<bool>> LogoutAsync(string userId);
    public Task<Result<bool>> SendResetPasswordTokenAsync(string email);
    public Task<Result<bool>> ResetPasswordAsync(string email, string newPassword, string token);
    public Task<Result<Session>> RefreshAccessTokenAsync(string email, string refreshToken);
}
