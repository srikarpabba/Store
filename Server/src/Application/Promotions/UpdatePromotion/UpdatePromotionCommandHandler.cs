using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products;
using Domain.Products;
using Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Promotions.UpdatePromotion;

internal sealed class UpdatePromotionCommandHandler(
    IApplicationDbContext context,
    ProductValidator productValidator)
    : ICommandHandler<UpdatePromotionCommand>
{
    public async Task<Result> Handle(UpdatePromotionCommand command, CancellationToken cancellationToken)
    {
        Promotion? promotion = await context.Promotions
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (promotion is null)
        {
            return Result.Failure(PromotionErrors.NotFound(command.Id));
        }

        if (command.ProductId.HasValue)
        {
            bool productExists = await context.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == command.ProductId.Value, cancellationToken);

            if (!productExists)
            {
                return Result.Failure(ProductErrors.NotFound(command.ProductId.Value));
            }
        }
        else
        {
            Result brandResult = await productValidator.ValidateBrandAsync(command.BrandId!.Value, cancellationToken);

            if (brandResult.IsFailure)
            {
                return brandResult;
            }
        }

        promotion.Update(
            command.Name.Trim(),
            command.DiscountPercentage,
            command.StartsAtUtc,
            command.EndsAtUtc,
            command.IsActive,
            command.ProductId,
            command.BrandId);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
