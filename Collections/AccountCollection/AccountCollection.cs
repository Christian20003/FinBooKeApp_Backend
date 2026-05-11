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

    public async Task<IdentityResult> CreateAccountAsync(UserAccount user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> DeleteAccountRefreshToken(UserAccount user)
    {
        return await _userManager.RemoveAuthenticationTokenAsync(
            user,
            LOGIN_PROVIDER,
            REFRESH_TOKEN_NAME
        );
    }

    public async Task<string> GeneratePasswordResetTokenAsync(UserAccount user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<UserAccount?> GetAccountAsync(Expression<Func<UserAccount, bool>> condition)
    {
        return await _userManager.Users.FirstOrDefaultAsync(condition);
    }

    public async Task<string?> GetAccountRefreshToken(UserAccount user)
    {
        return await _userManager.GetAuthenticationTokenAsync(
            user,
            LOGIN_PROVIDER,
            REFRESH_TOKEN_NAME
        );
    }

    public async Task<IdentityResult> ResetPasswordAsync(
        UserAccount user,
        string token,
        string password
    )
    {
        return await _userManager.ResetPasswordAsync(user, token, password);
    }

    public async Task<IdentityResult> SetAccountRefreshToken(UserAccount user, string refreshToken)
    {
        return await _userManager.SetAuthenticationTokenAsync(
            user,
            LOGIN_PROVIDER,
            REFRESH_TOKEN_NAME,
            refreshToken
        );
    }

    public async Task<IdentityResult> UpdateAccountAsync(UserAccount user)
    {
        return await _userManager.UpdateAsync(user);
    }
}
