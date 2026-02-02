using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// This method proofs if the provided email is a valid email address.
    /// </summary>
    /// <param name="email">
    /// The email value that should be verified.
    /// </param>
    /// <returns>
    /// <c>True</c> if the provided email is valid, otherwise <c>false</c>.
    /// </returns>
    private bool VerifyEmail(string email)
    {
        // TODO: User needs to verify his address
        LogEmailValidation(email);
        var emailValidator = new EmailAddressAttribute();
        return emailValidator.IsValid(email);
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Authentication: Verify email: {Email}")]
    private partial void LogEmailValidation(string email);
}
