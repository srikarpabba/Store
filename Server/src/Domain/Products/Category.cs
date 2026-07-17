using SharedKernel;

namespace Domain.Products;

public sealed class Category : AuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<CategoryGender> CategoryGenders { get; set; } = [];

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public CategoryGender AddGender(Guid genderId)
    {
        var categoryGender = new CategoryGender { GenderId = genderId };

        CategoryGenders.Add(categoryGender);

        return categoryGender;
    }

    public void RemoveGender(CategoryGender gender)
    {
        CategoryGenders.Remove(gender);
    }
}
