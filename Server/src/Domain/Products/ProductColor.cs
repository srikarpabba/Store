using SharedKernel;

namespace Domain.Products;

public sealed class ProductColor : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; private set; } = null!;
    public Guid ColorId { get; set; }
    public Color Color { get; private set; } = null!;
    public ICollection<ProductPhoto> Photos { get; set; } = [];
    public ICollection<ProductVariant> Variants { get; set; } = [];
}

