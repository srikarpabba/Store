using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Promotions.GetPromotions;

internal sealed class GetPromotionsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetPromotionsQuery, IReadOnlyList<PromotionResponse>>
{
    public async Task<Result<IReadOnlyList<PromotionResponse>>> Handle(
        GetPromotionsQuery query,
        CancellationToken cancellationToken)
    {
        List<PromotionResponse> items = await context.Promotions
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOnUtc)
            .Select(p => new PromotionResponse(
                p.Id,
                p.Name,
                p.DiscountPercentage,
                p.StartsAtUtc,
                p.EndsAtUtc,
                p.IsActive,
                p.ProductId,
                p.Product == null ? null : p.Product.Name,
                p.BrandId,
                p.Brand == null ? null : p.Brand.Name))
            .ToListAsync(cancellationToken);

        return items;
    }
}
