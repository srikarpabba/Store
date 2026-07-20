namespace Application.Common.Constants;

public static class Storefronts
{
    public const string Men = "men";
    public const string Women = "women";
    public const string New = "new";
    public const string Sale = "sale";

    // Mirrors the client's routable shop sections (ShopSection enum) —
    // every storefront a banner can target actually has a page to show it.
    private static readonly string[] All = [Men, Women, New, Sale];

    public static bool IsValid(string? storefront) => Normalize(storefront) is not null;

    /// <summary>
    /// Returns the canonical lowercase value for a storefront regardless of
    /// input casing (e.g. "MEN" and "Men" both normalize to "men"), or null
    /// if the input isn't a recognized storefront.
    /// </summary>
    public static string? Normalize(string? storefront)
    {
        if (string.IsNullOrWhiteSpace(storefront))
        {
            return null;
        }

        string trimmed = storefront.Trim();

        return Array.Find(All, s => s.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
    }
}
