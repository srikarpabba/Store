using SharedKernel;

namespace Domain.Products;

public sealed class Brand : AuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsFeatured { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
