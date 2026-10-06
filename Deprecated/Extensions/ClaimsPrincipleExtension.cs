using System.Security.Claims;
using System.Security.Principal;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IPrincipal"/> to centralize access to identity claims.
/// </summary>
public static class ClaimsPrincipleExtension
{
    /// <summary>
    /// Extracts the user id from the <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// </summary>
    /// <param name="principal">
    /// The security principal containing the user identity.
    /// </param>
    /// <returns>
    /// The user identifier as a <see cref="Guid"/>.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// Thrown if the identity is invalid, the name identifier claim is missing,
    /// or the claim value is not a valid <see cref="Guid"/>.
    /// </exception>
    public static Guid GetUserId(this IPrincipal principal)
    {
        var identity =
            principal.Identity as ClaimsIdentity
            ?? throw new AuthorizationException("ClaimsIdentity is null");
        var claim =
            identity.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new AuthorizationException("Claim is null");
        try
        {
            return Guid.Parse(claim.Value);
        }
        catch (FormatException exception)
        {
            throw new AuthorizationException("Claim value is not a valid Guid", exception);
        }
    }
}
