using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.DeleteProductImage;

internal sealed class DeleteProductImageCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<DeleteProductImageCommand>
{
    public async Task<Result> Handle(
        DeleteProductImageCommand command,
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

        context.ProductPhotos.Remove(photo);

        // Keep exactly one main photo per color when the main one is removed
        if (photo.IsMain)
        {
            ProductPhoto? nextMain = await context.ProductPhotos
                .Where(x => x.ProductColorId == photo.ProductColorId && x.Id != photo.Id)
                .OrderBy(x => x.CreatedOnUtc)
                .FirstOrDefaultAsync(cancellationToken);

            nextMain?.SetAsMain();
        }

        await context.SaveChangesAsync(cancellationToken);

        // Best effort — the record is gone either way, and an orphaned blob
        // is preferable to a photo whose file was deleted first
        try
        {
            await fileStorage.DeleteAsync(photo.FileName, cancellationToken);
        }
        catch (Exception)
        {
            // swallow: blob cleanup can be retried out of band
        }

        return Result.Success();
    }
}
