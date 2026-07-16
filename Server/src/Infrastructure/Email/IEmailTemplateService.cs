namespace Infrastructure.Email;

/// <summary>Renders the HTML bodies for outgoing emails from embedded templates.</summary>
public interface IEmailTemplateService
{
    string BuildConfirmationEmail(string firstName, string confirmationLink);
    string BuildPasswordResetEmail(string firstName, string resetLink);
}
