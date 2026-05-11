using FinBookeAPI.Models.Database.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FinBookeAPI.Models.Wrapper;

public interface IAccountManager
{
    /// <summary>
    /// This methods returns an enumerator that allows to iterate asynchronally over
    /// all stored user accounts in the authentication database.
    /// </summary>
    /// <returns>
    /// Enumerator over all list of user accounts.
    /// </returns>
    IAsyncEnumerable<UserAccount> GetUsersAsync();

    /// <summary>
    /// This method updates a user account (except password).
    /// </summary>
    /// <param name="user">
    /// The user account with all updated values
    /// </param>
    /// <returns>
    /// Flag if the requested operation has been successfully executed.
    /// </returns>
    Task<IdentityResult> UpdateUserAsync(UserAccount user);

    /// <summary>
    /// This method creates a new user account.
    /// </summary>
    /// <param name="user">
    /// The user account object (password will be ignored).
    /// </param>
    /// <param name="password">
    /// The password of the new user account.
    /// </param>
    /// <returns>
    /// Flag if the requested operation has been successfully executed.
    /// </returns>
    Task<IdentityResult> CreateUserAsync(UserAccount user, string password);

    /// <summary>
    /// This method replaces the current password with the provided one
    /// of the given user account.
    /// </summary>
    /// <param name="user">
    /// User account which password should be updated.
    /// </param>
    /// <param name="password">
    /// The new password.
    /// </param>
    /// <returns>
    /// Flag if the requested operation has been successfully executed.
    /// </returns>
    Task<IdentityResult> SetPasswordAsync(UserAccount user, string password);
}
