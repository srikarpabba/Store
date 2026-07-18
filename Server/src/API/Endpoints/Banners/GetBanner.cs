using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Banners;
using Application.Banners.GetBanner;
using SharedKernel.Authorization;

namespace API.Endpoints.Banners;

internal sealed class GetBanner : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBannerQuery, BannerResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetBannerQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.BannersRead)
        .WithName(nameof(GetBanner))
        .WithSummary("Gets a single banner for editing.")
        .Produces<BannerResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
