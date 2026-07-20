using Domain.Products;
using SharedKernel;

namespace Domain.Promotions;

/// <summary>
/// A percentage-off sale scoped to exactly one product or one brand. When
/// several active promotions apply to the same product (its own plus its
/// brand's), the best (highest) discount wins for display — see
/// <see cref="Application.Promotions.PromotionQueryExtensions"/>.
/// </summary>
public sealed class Promotion : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal DiscountPercentage { get; private set; }
    public DateTime? StartsAtUtc { get; private set; }
    public DateTime? EndsAtUtc { get; private set; }
    public bool IsActive { get; private set; }

    public Guid? ProductId { get; private set; }
    public Product? Product { get; }

    public Guid? BrandId { get; private set; }
    public Brand? Brand { get; }

    private Promotion()
    {
    }

    public static Promotion Create(
        string name,
        decimal discountPercentage,
        DateTime? startsAtUtc,
        DateTime? endsAtUtc,
        bool isActive,
        Guid? productId,
        Guid? brandId)
    {
        if (productId.HasValue == brandId.HasValue)
        {
            throw new ArgumentException("A promotion must be scoped to exactly one of a product or a brand.");
        }

        return new Promotion
        {
            Name = name,
            DiscountPercentage = discountPercentage,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = endsAtUtc,
            IsActive = isActive,
            ProductId = productId,
            BrandId = brandId
        };
    }

    public void Update(
        string name,
        decimal discountPercentage,
        DateTime? startsAtUtc,
        DateTime? endsAtUtc,
        bool isActive,
        Guid? productId,
        Guid? brandId)
    {
        if (productId.HasValue == brandId.HasValue)
        {
            throw new ArgumentException("A promotion must be scoped to exactly one of a product or a brand.");
        }

        Name = name;
        DiscountPercentage = discountPercentage;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        IsActive = isActive;
        ProductId = productId;
        BrandId = brandId;
    }
}
