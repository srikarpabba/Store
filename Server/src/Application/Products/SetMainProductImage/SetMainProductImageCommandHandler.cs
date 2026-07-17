using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.SetMainProductImage;

internal sealed class SetMainProductImageCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<SetMainProductImageCommand>
{
    public async Task<Result> Handle(
        SetMainProductImageCommand command,
        CancellationToken cancellationToken)
    {
        ProductPhoto? photo = await context.ProductPhotos
            .FirstOrDefaultAsync(
                x => x.Id == command.PhotoId
                    && x.ProductColor.ProductId == command.ProductId,
                cancellationToken);

        if (photo is null)
        {
            return Result.Failure(ProductErrors.PhotoNotFound(command.PhotoId));
        }

        // The main flag is per color — demote siblings of the same color only
        List<ProductPhoto> siblings = await context.ProductPhotos
            .Where(x => x.ProductColorId == photo.ProductColorId && x.Id != photo.Id && x.IsMain)
            .ToListAsync(cancellationToken);

        foreach (ProductPhoto sibling in siblings)
        {
            sibling.RemoveAsMain();
        }

        photo.SetAsMain();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
