using SharedKernel;

namespace Domain.Products;

public sealed class Size : AuditableEntity
{
    public string Name { get; set; }
    public ICollection<ProductVariant> ProductVariants { get; } = [];
    public ICollection<CategorySize> CategorySizes { get; set; } = [];

    public void Update(string name)
    {
        Name = name;
    }
}
