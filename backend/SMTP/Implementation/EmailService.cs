using System.Net;
using System.Net.Mail;
using learnyx.SMTP.Interfaces;
using learnyx.Utilities.Constants;
using Microsoft.Extensions.Options;

namespace learnyx.SMTP.Implementation;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string message)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port);
        client.EnableSsl = _settings.EnableSsl;
        client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);

        using var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_settings.Username, _settings.DisplayName);
        mailMessage.Subject = subject;
        mailMessage.Body = message;
        mailMessage.IsBodyHtml = true;

        mailMessage.To.Add(to);
        await client.SendMailAsync(mailMessage);
    }
}