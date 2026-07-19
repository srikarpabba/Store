namespace Domain.Products;

public sealed class CategoryGender
{
    public Guid CategoryId { get; set; }
    public Category Category { get; private set; } = null!;
    public Guid GenderId { get; set; }
    public Gender Gender { get; private set; } = null!;
    public string? PhotoFileName { get; private set; }

    /// <summary>
    /// Position of the category on this gender's storefront page — the same
    /// category can sit at different spots on /men and /women.
    /// </summary>
    public int SortOrder { get; private set; }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void SetPhoto(string fileName)
    {
        PhotoFileName = fileName;
    }

    public void RemovePhoto()
    {
        PhotoFileName = null;
    }
}
