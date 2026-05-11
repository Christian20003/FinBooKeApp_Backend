using FinBookeAPI.Models.Token;

namespace FinBookeAPI.Models.Database.Authentication;

/// <summary>
/// This class represents a user object sent to the client.
/// </summary>
public class User
{
    public Guid Id { get; set; } = new();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public JwtToken AccessToken { get; set; } = new();
    public JwtToken RefreshToken { get; set; } = new();
}
