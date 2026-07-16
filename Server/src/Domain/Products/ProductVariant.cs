using SharedKernel;

namespace Domain.Products;

public sealed class ProductVariant : AuditableEntity
{
    public Guid ProductColorId { get; private set; }
    public ProductColor ProductColor { get; private set; } = null!;
    public Guid SizeId { get; private set; }
    public Size Size { get; private set; } = null!;
    public string SKU { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int QuantityInStock { get; private set; }

    private ProductVariant()
    {
    }

    public static ProductVariant Create(
        ProductColor productColor,
        Guid sizeId,
        decimal price,
        int quantityInStock,
        string sku)
    {
        return new ProductVariant
        {
            ProductColor = productColor,
            ProductColorId = productColor.Id,
            SizeId = sizeId,
            Price = price,
            QuantityInStock = quantityInStock,
            SKU = sku
        };
    }
}
