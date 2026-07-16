namespace Application.Abstractions.Email;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string firstName, string confirmationLink, CancellationToken cancellationToken);
    Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink, CancellationToken cancellationToken);
}
