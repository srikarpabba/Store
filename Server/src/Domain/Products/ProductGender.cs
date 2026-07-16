namespace Domain.Products;

public sealed class ProductGender
{
    public Guid ProductId { get; set; }
    public Product Product { get; private set; } = null!;
    public Guid GenderId { get; set; }
    public Gender Gender { get; private set; } = null!;
}
