using System.Security.Authentication;
using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// This method proofs if the provided password corresponds to the user account and is valid.
    /// </summary>
    /// <param name="user">
    /// The user account.
    /// </param>
    /// <param name="password">
    /// The password received from a client.
    /// </param>
    /// <exception cref="InvalidCredentialException">
    /// If the provided password does not match with the stored one.
    /// </exception>
    /// <exception cref="ResourceLockedException">
    /// If user account is locked for further login attempts.
    /// </exception>
    private async Task VerifyPassword(UserAccount user, string password)
    {
        LogPasswordValidation(user.Id);
        var check = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true
        );
        if (check == SignInResult.Failed)
        {
            LogInvalidPassword(user.Id);
            throw new InvalidCredentialException("Invalid credentials");
        }
        else if (check == SignInResult.LockedOut)
        {
            LogLockedAccount(user.Id);
            throw new ResourceLockedException("User account is temporarily locked.");
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Authentication: Verify password of user - {UserId}"
    )]
    private partial void LogPasswordValidation(string userId);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidCredentials,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid password for user - {UserId}"
    )]
    private partial void LogInvalidPassword(string userId);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationLockedAccount,
        Level = LogLevel.Error,
        Message = "Authentication: Locked user account of - {UserId}"
    )]
    private partial void LogLockedAccount(string userId);
}
