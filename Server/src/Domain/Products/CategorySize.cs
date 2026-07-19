namespace Domain.Products;

public sealed class CategorySize
{
    public Guid CategoryId { get; set; }
    public Category Category { get; private set; } = null!;
    public Guid SizeId { get; set; }
    public Size Size { get; private set; } = null!;
}
