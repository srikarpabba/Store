using SharedKernel;

namespace Domain.Products;

public sealed class Color : AuditableEntity
{
    public string Name { get; set; }
    public string HexCode { get; set; }
    public ICollection<ProductColor> ProductColors { get; set; } = [];

    public void Update(string name, string hexCode)
    {
        Name = name;
        HexCode = hexCode;
    }
}
