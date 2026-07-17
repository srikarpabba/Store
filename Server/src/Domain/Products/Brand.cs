using SharedKernel;

namespace Domain.Products;

public sealed class Brand : AuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? LogoFileName { get; set; }
    public bool IsFeatured { get; set; }
    public ICollection<Product> Products { get; set; } = [];

    public void Update(string name, string? description, bool isFeatured)
    {
        Name = name;
        Description = description;
        IsFeatured = isFeatured;
    }

    public void SetLogo(string fileName)
    {
        LogoFileName = fileName;
    }

    public void RemoveLogo()
    {
        LogoFileName = null;
    }
}
