namespace Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "EmailConfiguration";
    public required string From { get; init; }
    public required string Host { get; init; }
    public int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string DisplayName { get; init; }
}
