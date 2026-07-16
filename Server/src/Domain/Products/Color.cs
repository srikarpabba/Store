using SharedKernel;

namespace Domain.Products;

public sealed class Color : BaseLookupEntity
{
    public string HexCode { get; set; }
    public ICollection<ProductColor> ProductColors { get; set; } = [];
}
