using SharedKernel;

namespace Domain.Banners;

public sealed class Banner : AuditableEntity
{
    /// <summary>Which storefront page shows this banner (e.g. "men", "women", "new", "sale").</summary>
    public string Storefront { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    public string? ImageFileName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public void Update(
        string storefront,
        string? title,
        string? link,
        int sortOrder,
        bool isActive)
    {
        Storefront = storefront;
        Title = title;
        LinkUrl = link;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void SetImage(string fileName)
    {
        ImageFileName = fileName;
    }
}
