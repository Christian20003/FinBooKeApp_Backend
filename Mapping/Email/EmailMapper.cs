using FinBooKeAPI.Models.Logic.Email;
using FinBooKeAPI.Models.Settings;
using Microsoft.Extensions.Options;

namespace FinBooKeAPI.Mapping.Email;

public static class EmailMapper
{
    public static EmailPayload GetEmailPayload(
        IOptions<SmtpSettings> smtpSettings,
        string body,
        string email,
        string subject
    )
    {
        return new EmailPayload
        {
            Host = smtpSettings.Value.Host,
            Port = smtpSettings.Value.Port,
            Username = smtpSettings.Value.Username,
            Password = smtpSettings.Value.Password,
            From = smtpSettings.Value.Address,
            To = [email],
            Subject = subject,
            Body = body,
            IsHtml = true,
        };
    }
}
