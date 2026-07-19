using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.ReorderProductImages;

internal sealed class ReorderProductImagesCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<ReorderProductImagesCommand>
{
    public async Task<Result> Handle(
        ReorderProductImagesCommand command,
        CancellationToken cancellationToken)
    {
        List<ProductPhoto> photos = await context.ProductPhotos
            .Where(x => x.ProductColorId == command.ProductColorId
                && x.ProductColor.ProductId == command.ProductId)
            .ToListAsync(cancellationToken);

        if (photos.Count == 0)
        {
            return Result.Failure(Error.NotFound(
                "ProductColor.NotFound",
                $"Product color '{command.ProductColorId}' has no photos on product '{command.ProductId}'."));
        }

        // The new order must cover exactly this color's photos — no missing
        // ids, no strays, no duplicates.
        var orderedIds = command.PhotoIds.Distinct().ToList();

        if (orderedIds.Count != photos.Count
            || photos.Any(photo => !orderedIds.Contains(photo.Id)))
        {
            return Result.Failure(Error.Problem(
                "ProductPhotos.OrderMismatch",
                "The photo order must contain each of the color's photos exactly once."));
        }

        foreach (ProductPhoto photo in photos)
        {
            photo.SetSortOrder(orderedIds.IndexOf(photo.Id));
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
