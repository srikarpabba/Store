using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.UploadProductImages;

internal sealed class UploadProductImagesCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<UploadProductImagesCommand>
{
    public async Task<Result> Handle(
        UploadProductImagesCommand command,
        CancellationToken cancellationToken)
    {
        Product? product = await context.Products
            .Include(x => x.ProductColors)
                .ThenInclude(x => x.Photos)
            .FirstOrDefaultAsync(
                x => x.Id == command.ProductId,
                cancellationToken);

        if (product is null)
        {
            return Result.Failure(
                ProductErrors.NotFound(command.ProductId));
        }

        ProductColor? productColor = product.ProductColors
            .FirstOrDefault(x => x.Id == command.ProductColorId);

        if (productColor is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "ProductColor.NotFound",
                    $"Product color '{command.ProductColorId}' was not found."));
        }

        bool hasMainPhoto = productColor.Photos.Any(x => x.IsMain);

        bool firstImage = !hasMainPhoto;

        foreach (FileUpload file in command.Files)
        {
            string extension = Path.GetExtension(file.FileName);

            string objectKey =
                $"products/{product.Id}/{Guid.NewGuid()}{extension}";

            await fileStorage.UploadAsync(
                file,
                objectKey,
                cancellationToken);

            ProductPhoto photo = product.CreatePhoto(
                productColor,
                objectKey,
                firstImage);

            context.ProductPhotos.Add(photo);

            firstImage = false;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
