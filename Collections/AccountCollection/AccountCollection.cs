using System.Linq.Expressions;
using FinBookeAPI.Models.Database.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinBooKeAPI.Collections.AccountCollection;

public class AccountCollection(UserManager<UserAccount> userManager) : IAccountCollection
{
    private readonly UserManager<UserAccount> _userManager = userManager;

    private static readonly string LOGIN_PROVIDER = "Local";
    private static readonly string REFRESH_TOKEN_NAME = "RefreshToken";

    public Task<IdentityResult> CreateAccountAsync(UserAccount user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<IdentityResult> DeleteAccountRefreshTokenAsync(UserAccount user)
    {
        return _userManager.RemoveAuthenticationTokenAsync(
            user,
            LOGIN_PROVIDER,
            REFRESH_TOKEN_NAME
        );
    }

    public Task<string> GeneratePasswordResetTokenAsync(UserAccount user)
    {
        return _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public Task<UserAccount?> GetAccountAsync(Expression<Func<UserAccount, bool>> condition)
    {
        return _userManager.Users.FirstOrDefaultAsync(condition);
    }

    public Task<string?> GetAccountRefreshTokenAsync(UserAccount user)
    {
        return _userManager.GetAuthenticationTokenAsync(user, LOGIN_PROVIDER, REFRESH_TOKEN_NAME);
    }

    public Task<IdentityResult> ResetPasswordAsync(UserAccount user, string token, string password)
    {
        return _userManager.ResetPasswordAsync(user, token, password);
    }

    public Task<IdentityResult> SetAccountRefreshTokenAsync(UserAccount user, string refreshToken)
    {
        return _userManager.SetAuthenticationTokenAsync(
            user,
            LOGIN_PROVIDER,
            REFRESH_TOKEN_NAME,
            refreshToken
        );
    }

    public Task<IdentityResult> UpdateAccountAsync(UserAccount user)
    {
        return _userManager.UpdateAsync(user);
    }
}
