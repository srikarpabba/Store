using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.DeleteBrand;

internal sealed class DeleteBrandCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<DeleteBrandCommand>
{
    public async Task<Result> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        Brand? brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (brand is null)
        {
            return Result.Failure(BrandErrors.NotFound(command.Id));
        }

        bool inUse = await context.Products
            .AsNoTracking()
            .AnyAsync(p => p.BrandId == command.Id, cancellationToken);

        if (inUse)
        {
            return Result.Failure(BrandErrors.InUse);
        }

        string? logoFileName = brand.LogoFileName;

        context.Brands.Remove(brand);

        await context.SaveChangesAsync(cancellationToken);

        if (logoFileName is not null)
        {
            // best effort — the brand record is gone either way
            try
            {
                await fileStorage.DeleteAsync(logoFileName, cancellationToken);
            }
            catch (Exception)
            {
                // swallow: blob cleanup can be retried out of band
            }
        }

        return Result.Success();
    }
}
