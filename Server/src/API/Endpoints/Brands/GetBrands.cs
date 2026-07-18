using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands;
using Application.Brands.GetBrands;
using Application.Common.Pagination;

namespace API.Endpoints.Brands;

internal sealed class GetBrands : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetBrandsQuery query,
            IQueryHandler<GetBrandsQuery, PagedResponse<BrandResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(query, cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetBrands))
        .WithSummary("Lists brands.")
        .Produces<PagedResponse<BrandResponse>>();
    }
}
