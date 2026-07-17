using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands;
using Application.Brands.GetBrand;

namespace API.Endpoints.Brands;

internal sealed class GetBrand : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBrandQuery, BrandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetBrandQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetBrand))
        .WithSummary("Gets a single brand.")
        .Produces<BrandResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
