using System.Security.Claims;
using FinBooKeAPI.Models.Logic.Authentication;

namespace FinBooKeAPI.Logic.Authentication;

public interface ITokenProvider
{
    public AuthenticationToken CreateToken(CreateTokenPayload payload);

    public ClaimsPrincipal VerifyToken(VerifyTokenPayload payload);
}
