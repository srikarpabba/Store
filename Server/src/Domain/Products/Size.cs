using SharedKernel;

namespace Domain.Products;

public sealed class Size : AuditableEntity
{
    public string Name { get; set; }
    public ICollection<ProductVariant> ProductVariants { get; } = [];

    public void Update(string name)
    {
        Name = name;
    }
}
