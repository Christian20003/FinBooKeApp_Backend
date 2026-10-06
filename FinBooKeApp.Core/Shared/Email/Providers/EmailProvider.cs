using System.Net;
using System.Net.Mail;
using FinBooKeApp.Core.Shared.Email.Interfaces;
using FinBooKeApp.Core.Shared.Email.Models;

namespace FinBooKeApp.Core.Shared.Email.Providers;

public class EmailProvider : IEmailProvider
{
    public void Send(EmailPayload payload)
    {
        var message = new MailMessage
        {
            From = new MailAddress(payload.From),
            Subject = payload.Subject,
            Body = payload.Body,
            IsBodyHtml = payload.IsHtml,
        };
        foreach (var email in payload.To)
        {
            message.To.Add(email);
        }
        var smtpClient = new SmtpClient
        {
            Host = payload.Host,
            Port = payload.Port,
            Credentials = new NetworkCredential(payload.Username, payload.Password),
            EnableSsl = true,
        };
        smtpClient.Send(message);
    }
}
