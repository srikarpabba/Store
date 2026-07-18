using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Banners;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Banners.DeleteBanner;

internal sealed class DeleteBannerCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<DeleteBannerCommand>
{
    public async Task<Result> Handle(DeleteBannerCommand command, CancellationToken cancellationToken)
    {
        Banner? banner = await context.Banners
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (banner is null)
        {
            return Result.Failure(BannerErrors.NotFound(command.Id));
        }

        string? imageFileName = banner.ImageFileName;

        context.Banners.Remove(banner);

        await context.SaveChangesAsync(cancellationToken);

        if (imageFileName is not null)
        {
            // best effort — the banner record is gone either way
            try
            {
                await fileStorage.DeleteAsync(imageFileName, cancellationToken);
            }
            catch (Exception)
            {
                // swallow: blob cleanup can be retried out of band
            }
        }

        return Result.Success();
    }
}
