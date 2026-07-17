using SharedKernel;

namespace Domain.Products;

public sealed class Gender : BaseLookupEntity
{
    public ICollection<ProductGender> ProductGenders { get; set; } = [];
    public ICollection<CategoryGender> CategoryGenders { get; set; } = [];
}
