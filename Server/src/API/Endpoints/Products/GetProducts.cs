using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Common.Pagination;
using Application.Products.GetProducts;

namespace API.Endpoints.Products;

internal sealed class GetProducts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetProductsQuery query,
            IQueryHandler<GetProductsQuery, PagedResponse<ProductResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(query, cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        });
    }
}
