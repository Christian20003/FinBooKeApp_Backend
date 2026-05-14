using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinBooKeAPI.Models.Logic.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FinBooKeAPI.Logic.Authentication;

public class TokenProvider : ITokenProvider
{
    private const string SIGNATURE_SCHEME = SecurityAlgorithms.HmacSha256Signature;

    public AuthenticationToken CreateToken(CreateTokenPayload payload)
    {
        var key = CreateSymmetricKey(payload.Secret);
        var credentials = new SigningCredentials(key, SIGNATURE_SCHEME);
        var subject = new ClaimsIdentity(payload.Claims);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = payload.Expires,
            Issuer = payload.Issuer,
            Audience = payload.Audience,
            SigningCredentials = credentials,
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new AuthenticationToken { Value = tokenString, Expires = payload.Expires.Ticks };
    }

    public ClaimsPrincipal VerifyToken(VerifyTokenPayload payload)
    {
        var key = CreateSymmetricKey(payload.Secret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParam = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = payload.Issuer,
            ValidAudience = payload.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero,
        };
        var principles = tokenHandler.ValidateToken(payload.Token, validationParam, out _);
        return principles;
    }

    private static SymmetricSecurityKey CreateSymmetricKey(string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        return new SymmetricSecurityKey(secretBytes);
    }
}
