using System.Net.Mail;
using System.Security.Authentication;
using FinBookeAPI.Models.Database.Authentication;
using FinBooKeAPI.Models.DTO.Authentication;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Models.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinBookeAPI.Services.Authentication;

public interface IAuthenticationService
{
    /// <summary>
    /// Tries to login a user with provided credentials to this API.
    /// </summary>
    /// <param name="loginData">The login credentials</param>
    /// <returns>The user account data if the login was successful</returns>
    public Task<Result<UserDTO>> LoginAsync(LoginDTO loginData);

    /// <summary>
    /// Tries to register a new user with provided credentials to this API.
    /// </summary>
    /// <param name="registerData">The data for registration</param>
    /// <returns> The user account data if the registration was successful</returns>
    public Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerData);

    /// <summary>
    /// This method sends a new generated access code to the provided email through an SMTP-Server and stores the result.
    /// </summary>
    /// <param name="email">
    /// The email address where the security code should be sent.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the provided email is not a valid email address.
    /// </exception>
    /// <exception cref="InvalidCredentialException">
    /// If the email is not assigned to any user account.
    /// </exception>
    /// <exception cref="ApplicationException">
    /// If configuration data to send emails is null or invalid.
    /// If the template email file is not found or locked.
    /// </exception>
    /// <exception cref="SmtpException">
    /// If the connection with the SMTP-Server failed as well as authentication.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// If the message object has been deleted, before sending.
    /// </exception>
    /// <exception cref="SmtpFailedRecipientException">
    /// If the message could not be sent to the client.
    /// </exception>
    public Task SendAccessCode(string email);

    /// <summary>
    /// This method resets the password of the user and sends the new random generated password via email to the user.
    /// </summary>
    /// <param name="email">
    /// The email address of the user account.
    /// </param>
    /// <param name="accessCode">
    /// The access code that the user has received with an email.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the provided email is not a valid email address.
    /// </exception>
    /// <exception cref="InvalidCredentialException">
    /// If the email is not assigned to any user account.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the accessCode is invalid or has expired.
    /// </exception>
    /// <exception cref="ApplicationException">
    /// If configuration data to send emails is null or invalid.
    /// If the template email file is not found or locked.
    /// </exception>
    /// <exception cref="SmtpException">
    /// If the connection with the SMTP-Server failed as well as authentication.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// If the message object has been deleted, before sending.
    /// </exception>
    /// <exception cref="SmtpFailedRecipientException">
    /// If the message could not be sent to the client.
    /// </exception>
    public Task ResetPassword(string email, string accessCode);

    /// <summary>
    /// This method generate a new access token for authentication, but only if the given
    /// refresh token is valid.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token.
    /// </param>
    /// <returns>
    /// The newly generated access token.
    /// </returns>
    /// <exception cref="ApplicationException">
    /// If required configuration data is null or the secrets to generate symmetric keys
    /// have less than 16 bytes.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// If the provided token is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If the provided token exceeds the maximum length or is revoked.
    /// </exception>
    /// <exception cref="SecurityTokenMalformedException">
    /// If the token does not fulfill the required structure.
    /// </exception>
    /// <exception cref="SecurityTokenEncryptionKeyNotFoundException">
    /// If the 'kid' header claim is not null AND decryption fails.
    /// </exception>
    /// <exception cref="SecurityTokenException">
    /// If the 'enc' header claim is null or empty.
    /// </exception>
    /// <exception cref="SecurityTokenExpiredException">
    /// If the refresh token has expired.
    /// </exception>
    /// <exception cref="SecurityTokenInvalidSignatureException">
    /// If the signature is not valid.
    /// </exception>
    /// <exception cref="SecurityTokenInvalidIssuerException">
    /// If the 'issuer' property in the token is invalid.
    /// </exception>
    /// <exception cref="SecurityTokenInvalidAudienceException">
    /// If the 'audience' property in the token is invalid.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the database operation has been canceled.
    /// </exception>
    //public Task<JwtToken> IssueJwtToken(string refreshToken);

    /// <summary>
    /// This method logs out a user by revoking both tokens.
    /// </summary>
    /// <param name="accessToken">
    /// The access token for authentication.
    /// </param>
    /// <param name="refreshToken">
    /// The refresh token to generate new access tokens.
    /// </param>
    /// <exception cref="ApplicationException">
    /// If required configuration data is null or the secrets to generate symmetric keys
    /// have less than 16 bytes.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// If a provided token is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If a provided token exceeds the maximum length.
    /// </exception>
    /// <exception cref="SecurityTokenMalformedException">
    /// If a token does not fulfill the required structure.
    /// </exception>
    /// <exception cref="SecurityTokenEncryptionKeyNotFoundException">
    /// If the 'kid' header claim is not null AND decryption fails.
    /// </exception>
    /// <exception cref="SecurityTokenException">
    /// If the 'enc' header claim is null or empty.
    /// </exception>
    /// <exception cref="SecurityTokenInvalidSignatureException">
    /// If a signature is not valid.
    /// </exception>
    /// <exception cref="SecurityTokenInvalidIssuerException">
    /// If the 'issuer' property in a token is invalid.
    /// </exception>
    /// <exception cref="SecurityTokenInvalidAudienceException">
    /// If the 'audience' property in a token is invalid.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the insertion operation in the database failed.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the database operation has been canceled.
    /// </exception>
    public Task Logout(string accessToken, string refreshToken);
}
