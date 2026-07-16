using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email;

internal sealed class SmtpEmailSender(
    IOptions<EmailOptions> options,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _emailOptions = options.Value;

    // Exceptions are deliberately not caught here: Hangfire records the
    // failure on the job and retries it with backoff.
    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailOptions.DisplayName, _emailOptions.From));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        logger.LogInformation("Sending email to {Recipient} with subject: {Subject}", toEmail, subject);

        await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, SecureSocketOptions.StartTls);
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        logger.LogInformation("Email sent successfully to {Recipient}", toEmail);
    }
}
