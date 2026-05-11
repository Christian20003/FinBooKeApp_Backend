using FinBookeAPI.Models.Database.Authentication;

namespace FinBookeAPI.Tests.Records;

public static class UserAccountRecord
{
    public static UserAccount GetObject()
    {
        return new UserAccount
        {
            Id = "85874677-bab2-4c08-b074-3f2927b7c23d",
            UserName = "Max Mustermann",
            Email = "max.mustermann@gmx.com",
            PasswordHash = "f5$z(kN)gTz9",
            ImagePath = "",
            AccessCode = "H78ER9",
            AccessCodeCreatedAt = DateTime.UtcNow,
            IsRevoked = false,
        };
    }
}
