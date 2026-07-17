using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.GetFilters;

namespace API.Endpoints.Products;

internal sealed class GetProductFilters : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/filters", async (
            IQueryHandler<GetProductFiltersQuery, ProductFiltersResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetProductFiltersQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        });
    }
}
