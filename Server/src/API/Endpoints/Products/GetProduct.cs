using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.GetProduct;

namespace API.Endpoints.Products;

internal sealed class GetProduct : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetProductQuery, ProductDetailsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProductQuery(id);

            return (await handler.Handle(query, cancellationToken))
                    .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetProduct));

    }
}
