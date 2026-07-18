using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Constants;
using Domain.Banners;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Banners.UpdateBanner;

internal sealed class UpdateBannerCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateBannerCommand>
{
    public async Task<Result> Handle(UpdateBannerCommand command, CancellationToken cancellationToken)
    {
        Banner? banner = await context.Banners
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (banner is null)
        {
            return Result.Failure(BannerErrors.NotFound(command.Id));
        }

        banner.Update(
            // the validator already confirmed this is a recognized storefront
            Storefronts.Normalize(command.Storefront)!,
            string.IsNullOrWhiteSpace(command.Title) ? null : command.Title.Trim(),
            string.IsNullOrWhiteSpace(command.Link) ? null : command.Link.Trim(),
            command.SortOrder,
            command.IsActive);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
