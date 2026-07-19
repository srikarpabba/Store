using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.GetFacets;

namespace API.Endpoints.Products;

internal sealed class GetProductFacets : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/facets", async (
            [AsParameters] GetProductFacetsQuery query,
            IQueryHandler<GetProductFacetsQuery, ProductFacetsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(query, cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetProductFacets))
        .WithSummary("Returns product counts per filter option for the current filter selection.")
        .WithDescription("Each facet's counts exclude that facet's own selection, so options within a facet stay additive while other facets narrow them.")
        .Produces<ProductFacetsResponse>();
    }
}
