using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products;
using Domain.Products;
using Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Promotions.CreatePromotion;

internal sealed class CreatePromotionCommandHandler(
    IApplicationDbContext context,
    ProductValidator productValidator)
    : ICommandHandler<CreatePromotionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePromotionCommand command, CancellationToken cancellationToken)
    {
        if (command.ProductId.HasValue)
        {
            bool productExists = await context.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == command.ProductId.Value, cancellationToken);

            if (!productExists)
            {
                return Result.Failure<Guid>(ProductErrors.NotFound(command.ProductId.Value));
            }
        }
        else
        {
            Result brandResult = await productValidator.ValidateBrandAsync(command.BrandId!.Value, cancellationToken);

            if (brandResult.IsFailure)
            {
                return Result.Failure<Guid>(brandResult.Error);
            }
        }

        var promotion = Promotion.Create(
            command.Name.Trim(),
            command.DiscountPercentage,
            command.StartsAtUtc,
            command.EndsAtUtc,
            command.IsActive,
            command.ProductId,
            command.BrandId);

        context.Promotions.Add(promotion);

        await context.SaveChangesAsync(cancellationToken);

        return promotion.Id;
    }
}
