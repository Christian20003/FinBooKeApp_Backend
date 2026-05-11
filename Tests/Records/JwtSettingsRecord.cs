using FinBookeAPI.Models.Settings;

namespace FinBookeAPI.Tests.Records;

public static class JwtSettingsRecord
{
    public static AuthenticationSettings GetObject()
    {
        return new AuthenticationSettings
        {
            Audience = "http://audience",
            Issuer = "http://issuer",
            AccessTokenSecret = "Por73MjWFc7s5ind78k4AcXEAEtxs0x1k6dZvDHoIUqGzwRDTyUubnGrCeDyZiy1",
            RefreshTokenSecret = "Por73MjWFc7s5ind78k4AcXEAEtxs0x1k6dZvDHoIUqGzwRDTyUubnGrCeDyZiy1",
        };
    }
}
