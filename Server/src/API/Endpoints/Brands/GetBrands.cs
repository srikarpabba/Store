using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands;
using Application.Brands.GetBrands;

namespace API.Endpoints.Brands;

internal sealed class GetBrands : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetBrandsQuery, IReadOnlyList<BrandResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetBrandsQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetBrands))
        .WithSummary("Lists all brands.")
        .Produces<IReadOnlyList<BrandResponse>>();
    }
}
