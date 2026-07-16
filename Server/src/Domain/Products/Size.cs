using SharedKernel;

namespace Domain.Products;

public sealed class Size : BaseLookupEntity
{
    public ICollection<ProductVariant> ProductVariants { get; } = [];
}
