using Microsoft.AspNetCore.Identity;

namespace FinBooKeAPI.Models.Database.Account;

public class UserAccount : IdentityUser
{
    // Properties name, email and password are already implemented in base class

    public string EmailHash { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public ThemeType ThemeType { get; set; } = ThemeType.LIGHT;
    public LanguageType FrontendLanguage { get; set; } = LanguageType.EN;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
