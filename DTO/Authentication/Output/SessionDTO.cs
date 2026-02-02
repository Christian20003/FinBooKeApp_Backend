using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Token;

namespace FinBookeAPI.DTO.Authentication.Output;

/// <summary>
/// This class models the transfer object of authentication data.
/// </summary>
public class SessionDTO
{
    public string AccessToken { get; set; }

    public long AccessTokenExpires { get; set; }

    public string RefreshToken { get; set; }

    public long RefreshTokenExpires { get; set; }

    public SessionDTO(User user)
    {
        AccessToken = user.AccessToken.Value;
        AccessTokenExpires = user.AccessToken.Expires;
        RefreshToken = user.RefreshToken.Value;
        RefreshTokenExpires = user.RefreshToken.Expires;
    }

    public SessionDTO(JwtToken accessToken, JwtToken refreshToken)
    {
        AccessToken = accessToken.Value;
        AccessTokenExpires = accessToken.Expires;
        RefreshToken = refreshToken.Value;
        RefreshTokenExpires = refreshToken.Expires;
    }
}
