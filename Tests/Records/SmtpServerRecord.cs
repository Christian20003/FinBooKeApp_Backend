using FinBooKeAPI.Models.Settings;

namespace FinBookeAPI.Tests.Records;

public static class SmtpSettingsRecord
{
    public static SmtpSettings GetObject()
    {
        return new SmtpSettings
        {
            Host = "http://host",
            Username = "max",
            Password = "1234",
            Address = "max.mustermann@gmx.de",
        };
    }
}
