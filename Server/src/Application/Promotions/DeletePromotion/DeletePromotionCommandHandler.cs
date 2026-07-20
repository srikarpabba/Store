using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Promotions.DeletePromotion;

internal sealed class DeletePromotionCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeletePromotionCommand>
{
    public async Task<Result> Handle(DeletePromotionCommand command, CancellationToken cancellationToken)
    {
        Promotion? promotion = await context.Promotions
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (promotion is null)
        {
            return Result.Failure(PromotionErrors.NotFound(command.Id));
        }

        context.Promotions.Remove(promotion);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
