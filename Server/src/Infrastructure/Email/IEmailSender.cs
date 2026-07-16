namespace Infrastructure.Email;

/// <summary>
/// Delivers a single, already-rendered email. Runs inside a Hangfire job,
/// so the signature uses only serializable primitives.
/// </summary>
public interface IEmailSender
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
}
