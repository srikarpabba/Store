using SharedKernel;

namespace Domain.Products;

public sealed class ProductPhoto : AuditableEntity
{
    public Guid ProductColorId { get; private set; }

    public ProductColor ProductColor { get; private set; } = null!;

    public string FileName { get; private set; } = string.Empty;

    public bool IsMain { get; private set; }

    /// <summary>
    /// Position within the color's slideshow. The main photo always
    /// displays first regardless of its sort order.
    /// </summary>
    public int SortOrder { get; private set; }

    private ProductPhoto()
    {
    }

    public static ProductPhoto Create(
        Guid productColorId,
        string fileName,
        bool isMain = false,
        int sortOrder = 0)
    {
        return new ProductPhoto
        {
            ProductColorId = productColorId,
            FileName = fileName,
            IsMain = isMain,
            SortOrder = sortOrder
        };
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void SetAsMain()
    {
        IsMain = true;
    }

    public void RemoveAsMain()
    {
        IsMain = false;
    }
}
