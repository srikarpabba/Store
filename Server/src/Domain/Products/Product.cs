using SharedKernel;

namespace Domain.Products;

public sealed class Product : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public Guid BrandId { get; private set; }
    public Brand Brand { get; private set; } = null!;
    public decimal Rating { get; private set; }
    public ICollection<ProductColor> ProductColors { get; } = [];
    public ICollection<ProductGender> ProductGenders { get; } = [];
    public ICollection<ProductVariant> Variants { get; } = [];

    private Product()
    {
    }

    public static Product Create(
        string name,
        string description,
        Guid categoryId,
        Guid brandId)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            CategoryId = categoryId,
            BrandId = brandId,
            Rating = 0m
        };

        product.Raise(new ProductCreatedDomainEvent(product.Id));

        return product;
    }
    public ProductColor AddColor(Guid colorId)
    {
        ProductColor color = new()
        {
            ColorId = colorId
        };

        ProductColors.Add(color);

        return color;
    }

    public void AddGender(Guid genderId)
    {
        ProductGenders.Add(new ProductGender
        {
            GenderId = genderId
        });
    }

    public ProductVariant AddVariant(
        ProductColor productColor,
        Guid sizeId,
        decimal price,
        int quantityInStock,
        string sku)
    {
        var variant = ProductVariant.Create(
            productColor,
            sizeId,
            price,
            quantityInStock,
            sku);

        Variants.Add(variant);

        return variant;
    }

    public ProductPhoto CreatePhoto(
        ProductColor productColor,
        string fileName,
        bool isMain = false)
    {
        return ProductPhoto.Create(
        productColor.Id,
        fileName,
        isMain);
    }
}
