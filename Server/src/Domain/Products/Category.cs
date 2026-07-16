using SharedKernel;

namespace Domain.Products;

public sealed class Category : AuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}

