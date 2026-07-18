using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Banners;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Banners.UploadBannerImage;

internal sealed class UploadBannerImageCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<UploadBannerImageCommand>
{
    public async Task<Result> Handle(UploadBannerImageCommand command, CancellationToken cancellationToken)
    {
        Banner? banner = await context.Banners
            .FirstOrDefaultAsync(b => b.Id == command.BannerId, cancellationToken);

        if (banner is null)
        {
            return Result.Failure(BannerErrors.NotFound(command.BannerId));
        }

        string? previousImage = banner.ImageFileName;

        string extension = Path.GetExtension(command.File.FileName);
        string objectKey = $"banners/{banner.Id}/{Guid.NewGuid()}{extension}";

        await fileStorage.UploadAsync(command.File, objectKey, cancellationToken);

        banner.SetImage(objectKey);

        await context.SaveChangesAsync(cancellationToken);

        if (previousImage is not null)
        {
            // best effort — the new image is already saved either way
            try
            {
                await fileStorage.DeleteAsync(previousImage, cancellationToken);
            }
            catch (Exception)
            {
                // swallow: blob cleanup can be retried out of band
            }
        }

        return Result.Success();
    }
}
