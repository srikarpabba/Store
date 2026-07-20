using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Promotions.CreatePromotionBatch;

internal sealed class CreatePromotionBatchCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreatePromotionBatchCommand, IReadOnlyList<Guid>>
{
    public async Task<Result<IReadOnlyList<Guid>>> Handle(
        CreatePromotionBatchCommand command,
        CancellationToken cancellationToken)
    {
        var productIds = command.Items
            .Where(i => i.ProductId.HasValue)
            .Select(i => i.ProductId!.Value)
            .Distinct()
            .ToList();

        var brandIds = command.Items
            .Where(i => i.BrandId.HasValue)
            .Select(i => i.BrandId!.Value)
            .Distinct()
            .ToList();

        if (productIds.Count > 0)
        {
            int existingProducts = await context.Products
                .AsNoTracking()
                .CountAsync(p => productIds.Contains(p.Id), cancellationToken);

            if (existingProducts != productIds.Count)
            {
                return Result.Failure<IReadOnlyList<Guid>>(Error.NotFound(
                    "Products.NotFound",
                    "One or more products were not found."));
            }
        }

        if (brandIds.Count > 0)
        {
            int existingBrands = await context.Brands
                .AsNoTracking()
                .CountAsync(b => brandIds.Contains(b.Id), cancellationToken);

            if (existingBrands != brandIds.Count)
            {
                return Result.Failure<IReadOnlyList<Guid>>(Error.NotFound(
                    "Brands.NotFound",
                    "One or more brands were not found."));
            }
        }

        string name = command.Name.Trim();

        var promotions = command.Items
            .Select(item => Promotion.Create(
                name,
                item.DiscountPercentage,
                item.StartsAtUtc,
                item.EndsAtUtc,
                item.IsActive,
                item.ProductId,
                item.BrandId))
            .ToList();

        context.Promotions.AddRange(promotions);

        await context.SaveChangesAsync(cancellationToken);

        return promotions.Select(p => p.Id).ToList();
    }
}
