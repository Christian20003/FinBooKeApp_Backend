using System.Security.Claims;
using System.Security.Principal;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.AppConfig.Authentication;

public static class PrincipalExtension
{
    public static string GetClaimValue(this IPrincipal principal, string claimType)
    {
        var identity = principal.Identity as ClaimsIdentity;
        var value =
            identity?.FindFirst(claimType)?.Value
            ?? throw new AuthorizationException("Unauthorized access on resource");
        return value;
    }

    public static Guid GetUserId(this IPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        var value =
            identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new AuthorizationException("Unauthorized access on resource");
        try
        {
            return Guid.Parse(value);
        }
        catch (FormatException exception)
        {
            throw new AuthorizationException("Invalid user id", exception);
        }
    }
}
