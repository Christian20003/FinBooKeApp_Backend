using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.DTO.Authentication.Input;

/// <summary>
/// This class represents a transfer object to refresh access token requests.
/// </summary>
public class RefreshAccessTokenDTO
{
    /// <summary>
    /// The refresh token to verify it's authorization.
    /// </summary>
    [Required(ErrorMessage = "Refresh token is missing")]
    public string RefreshToken { get; set; } = "";

    [Required(ErrorMessage = "Expire value is missing")]
    [Range(1, Int64.MaxValue, ErrorMessage = "Expire value must be larger than 0")]
    public long RefreshTokenExpires { get; set; }
}
