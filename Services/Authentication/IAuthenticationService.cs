using FinBooKeAPI.Models.DTO.Authentication;
using FinBookeAPI.Models.Result;

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
    /// Tries to logout a authenticated user.
    /// </summary>
    /// <param name="userId">The id of the user who is authenticated</param>
    /// <returns>True if the user could be logged out</returns>
    public Task<Result<bool>> LogoutAsync(string userId);

    /// <summary>
    /// Sends a reset password token to the given email address.
    /// </summary>
    /// <param name="email">The email address where the token should be send</param>
    /// <returns>True if the email could be send</returns>
    public Task<Result<bool>> SendResetPasswordTokenAsync(string email);

    /// <summary>
    /// Resets the password of a user account.
    /// </summary>
    /// <param name="resetData">The data to reset the password.</param>
    /// <returns>True if the reset operation was successful</returns>
    public Task<Result<bool>> ResetPasswordAsync(ResetPasswordDTO resetData);

    /// <summary>
    /// Returns a new access token.
    /// </summary>
    /// <param name="refreshData">The data to generate a new access token</param>
    /// <returns>The new access token</returns>
    public Task<Result<SessionDTO>> RefreshAccessTokenAsync(RefreshAccessTokenDTO refreshData);
}
