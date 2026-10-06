using System.Security.Claims;
using FinBooKeAPI.Models.Logic.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FinBooKeAPI.Logic.Authentication;

/// <summary>
/// Handles the generation and verification of json web tokens (JWT).
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Generates a json web token (JWT).
    /// </summary>
    /// <param name="payload">Payload to generate a JWT</param>
    /// <returns>The generated JWT with its expiration time</returns>
    public AuthenticationToken CreateToken(CreateTokenPayload payload);

    /// <summary>
    /// Verifies a given json web token (JWT).
    /// </summary>
    /// <param name="payload">Payload to verify a JWT</param>
    /// <returns>The claims stored inside the JWT</returns>
    /// <exception cref="SecurityTokenMalformedException">If the given token is not a JWT</exception>
    /// <exception cref="SecurityTokenException">If the given token is invalid</exception>
    public ClaimsPrincipal VerifyToken(VerifyTokenPayload payload);
}
