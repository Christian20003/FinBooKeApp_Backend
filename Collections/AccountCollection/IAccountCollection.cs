using System.Linq.Expressions;
using FinBooKeAPI.Models.Database.Account;
using Microsoft.AspNetCore.Identity;

namespace FinBooKeAPI.Collections.AccountCollection;

public interface IAccountCollection
{
    public Task<IdentityResult> CreateAccountAsync(UserAccount user, string password);

    public Task<UserAccount?> GetAccountAsync(Expression<Func<UserAccount, bool>> condition);

    public Task<IdentityResult> UpdateAccountAsync(UserAccount user);

    public Task<string> GeneratePasswordResetTokenAsync(UserAccount user);

    public Task<IdentityResult> ResetPasswordAsync(UserAccount user, string token, string password);

    public Task<IdentityResult> SetAccountRefreshTokenAsync(UserAccount user, string refreshToken);

    public Task<string?> GetAccountRefreshTokenAsync(UserAccount user);

    public Task<IdentityResult> DeleteAccountRefreshTokenAsync(UserAccount user);

    public Task<string> GenerateChangeEmailToken(UserAccount user, string newEmail);

    public Task<IdentityResult> ChangeEmailAddressAsync(
        UserAccount user,
        string token,
        string newEmail
    );

    public Task<string> GenerateEmailVerificationToken(UserAccount user);

    public Task<IdentityResult> VerifyEmailAddressAsync(UserAccount user, string token);
}
