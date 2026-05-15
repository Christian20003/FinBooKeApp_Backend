using System.Net;
using System.Net.Mail;
using FinBooKeAPI.Models.Logic.Email;

namespace FinBooKeAPI.Logic.Email;

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
