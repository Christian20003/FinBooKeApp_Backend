using System.Security.Claims;
using FinBooKeAPI.Models.Logic.Authentication;
using FinBookeAPI.Models.Settings;
using Microsoft.Extensions.Options;

namespace FinBooKeAPI.Logic.Authentication;

public class TokenProvider(IOptions<AuthenticationSettings> settings) : ITokenProvider
{
    private readonly IOptions<AuthenticationSettings> _settings = settings;

    public AuthenticationToken GenerateToken(
        IEnumerable<Claim> claims,
        string secret,
        DateTime expires
    )
    {
        /* var audience = _settings.Value.Audience;
        var issuer = _settings.Value.Issuer;
        if (audience == null)
        {
            LogInvalidAudience(audience);
            throw new ApplicationException("Audience configuration is null");
        }
        if (issuer == null)
        {
            LogInvalidIssuer(issuer);
            throw new ApplicationException("Issuer configuration is null");
        }

        var bytes = Encoding.UTF8.GetBytes(secret);
        if (bytes.Length < 16)
        {
            LogInvalidSecret();
            throw new ApplicationException("Given secret is too small to generated symmetric key");
        }
        var key = new SymmetricSecurityKey(bytes);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256Signature
            ),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token); */
        throw new NotImplementedException();
    }

    public AuthenticationToken VerifyToken(string token, string secret)
    {
        throw new NotImplementedException();
    }
}
