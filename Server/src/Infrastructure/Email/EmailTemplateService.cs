using System.Globalization;
using System.Reflection;
using SharedKernel;

namespace Infrastructure.Email;

internal sealed class EmailTemplateService(IDateTimeProvider dateTimeProvider) : IEmailTemplateService
{
    private static readonly Assembly ResourceAssembly = typeof(EmailTemplateService).Assembly;

    public string BuildConfirmationEmail(string firstName, string confirmationLink)
    {
        return Render("ConfirmationEmail", new Dictionary<string, string>
        {
            ["FirstName"] = firstName,
            ["ConfirmationLink"] = confirmationLink,
            ["Year"] = dateTimeProvider.UtcNow.Year.ToString(CultureInfo.InvariantCulture)
        });
    }

    public string BuildPasswordResetEmail(string firstName, string resetLink)
    {
        return Render("PasswordReset", new Dictionary<string, string>
        {
            ["FirstName"] = firstName,
            ["ResetLink"] = resetLink,
            ["Year"] = dateTimeProvider.UtcNow.Year.ToString(CultureInfo.InvariantCulture)
        });
    }

    private static string Render(string templateName, Dictionary<string, string> placeholders)
    {
        string resourceName = $"{typeof(EmailTemplateService).Namespace}.EmailTemplates.{templateName}.html";

        using Stream stream = ResourceAssembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource {resourceName} not found");

        using var reader = new StreamReader(stream);
        string html = reader.ReadToEnd();

        foreach (KeyValuePair<string, string> placeholder in placeholders)
        {
            html = html.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value, StringComparison.Ordinal);
        }

        return html;
    }
}
