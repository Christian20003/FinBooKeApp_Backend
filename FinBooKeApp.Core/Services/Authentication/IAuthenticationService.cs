using FinBooKeApp.Core.Services.Authentication.Models;
using FinBookeApp.Core.Shared.Result;

namespace FinBooKeApp.Core.Services.Authentication;

public interface IAuthenticationService
{
    public Task<Result<User, AuthenticationServiceResultCode>> LoginAsync(
        string email,
        string password
    );
    public Task<Result<User, AuthenticationServiceResultCode>> RegisterAsync(
        string email,
        string username,
        string password
    );
    public Task<Result<bool, AuthenticationServiceResultCode>> LogoutAsync(string userId);
    public Task<Result<bool, AuthenticationServiceResultCode>> SendResetPasswordTokenAsync(
        string email
    );
    public Task<Result<bool, AuthenticationServiceResultCode>> ResetPasswordAsync(
        string email,
        string newPassword,
        string token
    );
    public Task<Result<Session, AuthenticationServiceResultCode>> RefreshAccessTokenAsync(
        string email,
        string refreshToken
    );
}
