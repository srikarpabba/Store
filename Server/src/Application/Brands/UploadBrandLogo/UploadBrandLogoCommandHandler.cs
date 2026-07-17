using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.UploadBrandLogo;

internal sealed class UploadBrandLogoCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<UploadBrandLogoCommand>
{
    public async Task<Result> Handle(UploadBrandLogoCommand command, CancellationToken cancellationToken)
    {
        Brand? brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.BrandId, cancellationToken);

        if (brand is null)
        {
            return Result.Failure(BrandErrors.NotFound(command.BrandId));
        }

        string? previousLogo = brand.LogoFileName;

        string extension = Path.GetExtension(command.File.FileName);
        string objectKey = $"brands/{brand.Id}/{Guid.NewGuid()}{extension}";

        await fileStorage.UploadAsync(command.File, objectKey, cancellationToken);

        brand.SetLogo(objectKey);

        await context.SaveChangesAsync(cancellationToken);

        if (previousLogo is not null)
        {
            // best effort — the new logo is already saved either way
            try
            {
                await fileStorage.DeleteAsync(previousLogo, cancellationToken);
            }
            catch (Exception)
            {
                // swallow: blob cleanup can be retried out of band
            }
        }

        return Result.Success();
    }
}
