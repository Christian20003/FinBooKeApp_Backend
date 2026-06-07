using FinBooKeAPI.Logic.Security;
using FinBookeAPI.Models.Database.Authentication;
using FinBooKeAPI.Models.DTO.Authentication;
using FinBooKeAPI.Models.Logic.Authentication;

namespace FinBooKeAPI.Mapping.Authentication;

public static class UserMapper
{
    public static UserDTO GetUserDTO(
        UserAccount user,
        AuthenticationToken accessToken,
        AuthenticationToken refreshToken,
        IDataProtection protection
    )
    {
        return new UserDTO
        {
            Email = protection.UnprotectEmail(user.Email!),
            Name = user.UserName!,
            ImagePath = user.ImagePath,
            Session = new SessionDTO
            {
                AccessToken = accessToken.Value,
                RefreshToken = refreshToken.Value,
                AccessTokenExpiresAt = accessToken.Expires,
                RefreshTokenExpiresAt = refreshToken.Expires,
            },
        };
    }
}
