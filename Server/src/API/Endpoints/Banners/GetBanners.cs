using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Banners;
using Application.Banners.GetBanners;
using SharedKernel.Authorization;

namespace API.Endpoints.Banners;

internal sealed class GetBanners : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            string? storefront,
            IQueryHandler<GetBannersQuery, IReadOnlyList<BannerResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetBannersQuery(storefront), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.BannersRead)
        .WithName(nameof(GetBanners))
        .WithSummary("Lists banners for the admin dashboard.")
        .WithDescription("Optionally filtered by storefront (men/women/kids). Admin-only — the storefront-facing banner list comes from /storefronts/{storefront}.")
        .Produces<IReadOnlyList<BannerResponse>>();
    }
}
