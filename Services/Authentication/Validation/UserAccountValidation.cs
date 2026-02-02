using System.Security.Authentication;
using FinBookeAPI.Models.Authentication;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// This method proofs if any user account exist in the authentication database with the provided email address.
    /// </summary>
    /// <param name="email">
    /// The email address of the user account.
    /// </param>
    /// <returns>
    /// The user account that corresponds to the given email address.
    /// </returns>
    /// <exception cref="InvalidCredentialException">
    /// If the email address does not correspond to any user account.
    /// </exception>
    private async Task<UserAccount> VerifyUserAccount(string email)
    {
        LogUserAccountValidation(email);
        var accounts = _accountManager.GetUsersAsync();
        var user = await accounts.FirstOrDefaultAsync(account =>
            email == _protector.UnprotectEmail(account.Email!)
        );
        // Proof if user exist
        if (user == null)
        {
            LogInvalidEmail(email);
            throw new InvalidCredentialException("Invalid credentials");
        }
        return user;
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Authentication: Verify user account of - {Email}"
    )]
    private partial void LogUserAccountValidation(string email);
}
