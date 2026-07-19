using SharedKernel;

namespace Domain.Products;

public sealed class Subcategory : AuditableEntity
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; private set; } = null!;
    public ICollection<Product> Products { get; } = [];

    public void Update(string name, Guid categoryId)
    {
        Name = name;
        CategoryId = categoryId;
    }
}
