using Application.Abstractions.Email;
using Hangfire;

namespace Infrastructure.Email;

/// <summary>
/// Renders the email and enqueues delivery as a Hangfire background job,
/// so callers never wait on SMTP and failures are retried automatically.
/// </summary>
internal sealed class EmailService(
    IEmailTemplateService emailTemplateService,
    IBackgroundJobClient backgroundJobClient) : IEmailService
{
    public Task SendConfirmationEmailAsync(string email, string firstName, string confirmationLink, CancellationToken cancellationToken)
    {
        string htmlBody = emailTemplateService.BuildConfirmationEmail(firstName, confirmationLink);

        backgroundJobClient.Enqueue<IEmailSender>(sender =>
            sender.SendAsync(email, firstName, "Confirm your Store account", htmlBody));

        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink, CancellationToken cancellationToken)
    {
        string htmlBody = emailTemplateService.BuildPasswordResetEmail(firstName, resetLink);

        backgroundJobClient.Enqueue<IEmailSender>(sender =>
            sender.SendAsync(email, firstName, "Reset your Store password", htmlBody));

        return Task.CompletedTask;
    }
}
