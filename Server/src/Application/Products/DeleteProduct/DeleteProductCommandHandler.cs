using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.DeleteProduct;

internal sealed class DeleteProductCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        Product? product = await context.Products
            .FirstOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(command.Id));
        }

        bool inUseByPromotion = await context.Promotions
            .AsNoTracking()
            .AnyAsync(p => p.ProductId == command.Id, cancellationToken);

        if (inUseByPromotion)
        {
            return Result.Failure(ProductErrors.InUseByPromotion);
        }

        context.Products.Remove(product);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
