using System.Net.Mail;
using FinBooKeApp.Core.Shared.Email.Models;

namespace FinBooKeApp.Core.Shared.Email.Interfaces;

public interface IEmailProvider
{
    /// <summary>
    /// Sends an email to specified email addresses.
    /// </summary>
    /// <param name="payload">Data to send email through SMTP server</param>
    /// <exception cref="SmtpException">If access to the SMTP server failed</exception>
    /// <exception cref="SmtpFailedRecipientException">If the email could not be send to the primary address</exception>
    /// <exception cref="SmtpFailedRecipientsException">If the email could not be send to the secondary addresses</exception>
    public void Send(EmailPayload payload);
}
