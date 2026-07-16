using SharedKernel;

namespace Domain.Products;

public sealed class ProductPhoto : AuditableEntity
{
    public Guid ProductColorId { get; private set; }

    public ProductColor ProductColor { get; private set; } = null!;

    public string FileName { get; private set; } = string.Empty;

    public bool IsMain { get; private set; }

    private ProductPhoto()
    {
    }

    public static ProductPhoto Create(
        Guid productColorId,
        string fileName,
        bool isMain = false)
    {
        return new ProductPhoto
        {
            ProductColorId = productColorId,
            FileName = fileName,
            IsMain = isMain
        };
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
