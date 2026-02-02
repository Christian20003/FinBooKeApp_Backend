using System.Net;
using System.Net.Mail;
using FinBookeAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Services.Email;

public partial class EmailService(IOptions<SmtpServer> config, ILogger<EmailService> logger)
    : IEmailService
{
    private readonly IOptions<SmtpServer> _config = config;
    private readonly ILogger<EmailService> _logger = logger;

    public void Send(string email, string subject, string body, bool isHtml = true)
    {
        LogSendEmail();
        var host = _config.Value.Host;
        var from = _config.Value.Address;
        var port = _config.Value.Port;
        if (host == null)
        {
            LogInvalidHost(host);
            throw new ApplicationException("SMTP-Host cannot be null");
        }
        if (from == null)
        {
            LogInvalidSender(from);
            throw new ApplicationException("Address of the sender cannot be null");
        }
        if (port <= 0 || port > 65535)
        {
            LogInvalidPort(port);
            throw new ApplicationException("SMTP-Port must be between 1 and 65.535");
        }
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
            };
            message.To.Add(email);
            var smtpClient = new SmtpClient
            {
                Host = host,
                Port = port,
                Credentials = new NetworkCredential(_config.Value.Username, _config.Value.Password),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }
        catch (Exception exception)
            when (exception is ArgumentException || exception is FormatException)
        {
            LogInvalidEmail(email);
            throw new ApplicationException("Address of the recipient is not a valid email address");
        }
        LogEmailSend();
    }

    [LoggerMessage(
        EventId = LogEvents.EmailSend,
        Level = LogLevel.Information,
        Message = "Email: Send email to a user"
    )]
    private partial void LogSendEmail();

    [LoggerMessage(
        EventId = LogEvents.EmailInvalidHost,
        Level = LogLevel.Critical,
        Message = "Email: Host is invalid - {Host}"
    )]
    private partial void LogInvalidHost(string? host);

    [LoggerMessage(
        EventId = LogEvents.EmailInvalidPort,
        Level = LogLevel.Critical,
        Message = "Email: Port is invalid - {Port}"
    )]
    private partial void LogInvalidPort(int port);

    [LoggerMessage(
        EventId = LogEvents.EmailInvalidSender,
        Level = LogLevel.Critical,
        Message = "Email: Sender is invalid - {Sender}"
    )]
    private partial void LogInvalidSender(string? sender);

    [LoggerMessage(
        EventId = LogEvents.EmailInvalidEmail,
        Level = LogLevel.Error,
        Message = "Email: Invalid email - {Email}"
    )]
    private partial void LogInvalidEmail(string email);

    [LoggerMessage(
        EventId = LogEvents.EmailSendSuccess,
        Level = LogLevel.Information,
        Message = "Email: Email send successfully"
    )]
    private partial void LogEmailSend();
}
