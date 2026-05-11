using Microsoft.AspNetCore.Identity;

namespace FinBookeAPI.Models.Database.Authentication;

public class UserAccount : IdentityUser
{
    // Properties name, email and password are already implemented in base class

    public string ImagePath { get; set; } = "";
    public string? AccessCode { get; set; }
    public DateTime? AccessCodeCreatedAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
}
