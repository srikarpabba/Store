using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.DeleteBrandLogo;

internal sealed class DeleteBrandLogoCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<DeleteBrandLogoCommand>
{
    public async Task<Result> Handle(DeleteBrandLogoCommand command, CancellationToken cancellationToken)
    {
        Brand? brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.BrandId, cancellationToken);

        if (brand is null)
        {
            return Result.Failure(BrandErrors.NotFound(command.BrandId));
        }

        string? logoFileName = brand.LogoFileName;

        if (logoFileName is null)
        {
            return Result.Success();
        }

        brand.RemoveLogo();

        await context.SaveChangesAsync(cancellationToken);

        try
        {
            await fileStorage.DeleteAsync(logoFileName, cancellationToken);
        }
        catch (Exception)
        {
            // swallow: blob cleanup can be retried out of band
        }

        return Result.Success();
    }
}
