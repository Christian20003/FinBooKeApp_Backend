using System.Linq.Expressions;
using FinBookeAPI.Models.Database.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FinBooKeAPI.Collections.AccountCollection;

public interface IAccountCollection
{
    public Task<IdentityResult> CreateAccountAsync(UserAccount user, string password);

    public Task<UserAccount?> GetAccountAsync(Expression<Func<UserAccount, bool>> condition);

    public Task<IdentityResult> UpdateAccountAsync(UserAccount user);

    public Task<string> GeneratePasswordResetTokenAsync(UserAccount user);

    public Task<IdentityResult> ResetPasswordAsync(UserAccount user, string token, string password);

    public Task<IdentityResult> SetAccountRefreshToken(UserAccount user, string refreshToken);

    public Task<string?> GetAccountRefreshToken(UserAccount user);

    public Task<IdentityResult> DeleteAccountRefreshToken(UserAccount user);
}
