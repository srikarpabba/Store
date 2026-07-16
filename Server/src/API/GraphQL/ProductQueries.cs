using Application.Abstractions.Messaging;
using Application.Common.Pagination;
using Application.Products.GetProducts;

namespace API.GraphQL;

[ExtendObjectType(typeof(Query))]
public class ProductQueries
{
    public async Task<PagedResponse<ProductResponse>> Products(
        GetProductsQuery input,
        [Service] IQueryHandler<GetProductsQuery, PagedResponse<ProductResponse>> handler,
        CancellationToken cancellationToken)
    {
        return (await handler.Handle(input, cancellationToken))
        .ToGraphQl();
    }
}
