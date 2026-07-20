using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Promotions.GetPromotion;

internal sealed class GetPromotionQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetPromotionQuery, PromotionResponse>
{
    public async Task<Result<PromotionResponse>> Handle(GetPromotionQuery query, CancellationToken cancellationToken)
    {
        PromotionResponse? response = await context.Promotions
            .AsNoTracking()
            .Where(p => p.Id == query.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.Failure<PromotionResponse>(PromotionErrors.NotFound(query.Id));
        }

        return response;
    }
}
