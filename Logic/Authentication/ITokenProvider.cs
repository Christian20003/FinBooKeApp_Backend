using System.Security.Claims;
using FinBooKeAPI.Models.Logic.Authentication;

namespace FinBooKeAPI.Logic.Authentication;

public interface ITokenProvider
{
    public AuthenticationToken GenerateToken(
        IEnumerable<Claim> claims,
        string secret,
        DateTime expires
    );

    public AuthenticationToken VerifyToken(string token, string secret);
}
