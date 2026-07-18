using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Constants;
using Domain.Banners;
using SharedKernel;

namespace Application.Banners.CreateBanner;

internal sealed class CreateBannerCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateBannerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBannerCommand command, CancellationToken cancellationToken)
    {
        var banner = new Banner
        {
            // the validator already confirmed this is a recognized storefront
            Storefront = Storefronts.Normalize(command.Storefront)!,
            Title = string.IsNullOrWhiteSpace(command.Title) ? null : command.Title.Trim(),
            LinkUrl = string.IsNullOrWhiteSpace(command.Link) ? null : command.Link.Trim(),
            SortOrder = command.SortOrder,
            IsActive = command.IsActive
        };

        context.Banners.Add(banner);

        await context.SaveChangesAsync(cancellationToken);

        return banner.Id;
    }
}
