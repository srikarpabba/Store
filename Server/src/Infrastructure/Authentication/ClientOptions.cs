namespace Infrastructure.Authentication;

/// <summary>Where the SPA lives — used to build links sent in emails.</summary>
public sealed class ClientOptions
{
    public const string SectionName = "Client";
    public string BaseUrl { get; init; } = string.Empty;
}
