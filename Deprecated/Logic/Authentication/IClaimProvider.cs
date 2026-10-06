using System.Security.Claims;
using FinBooKeAPI.Models.Exceptions;

namespace FinBooKeAPI.Logic.Authentication;

/// <summary>
/// Handles creation of authentication token claims and extraction of claim information.
/// </summary>
public interface IClaimProvider
{
    /// <summary>
    /// Generates a list of claims which includes the given user id and user email.
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="email">The email address of the user</param>
    /// <returns>A list of claims with the user id and user email</returns>
    public IEnumerable<Claim> CreateClaims(string userId, string email);

    /// <summary>
    /// Extracts the user id from a given claim identity.
    /// </summary>
    /// <param name="claims">The claim identity of a user</param>
    /// <returns>The user id</returns>
    /// <exception cref="ClaimException">If the user id claim does not exist</exception>
    public string GetUserId(ClaimsPrincipal claims);

    /// <summary>
    /// Extracts the users email address from a given claim identity.
    /// </summary>
    /// <param name="claims">The claim identity of a user</param>
    /// <returns>The users email address</returns>
    /// <exception cref="ClaimException">If the email address does not exist</exception>
    public string GetEmail(ClaimsPrincipal claims);

    /// <summary>
    /// Extracts the expiration date from a given claim identity.
    /// </summary>
    /// <param name="claims">The claim identity of a user</param>
    /// <returns>The expiration date</returns>
    /// <exception cref="ClaimException">If the expiration date does not exist</exception>
    public DateTime GetExpires(ClaimsPrincipal claims);
}
