using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories;
using Application.Categories.GetCategories;
using Application.Common.Pagination;

namespace API.Endpoints.Categories;

internal sealed class GetCategories : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetCategoriesQuery query,
            IQueryHandler<GetCategoriesQuery, PagedResponse<CategoryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(query, cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetCategories))
        .WithSummary("Lists categories with their gender tags and photos.")
        .Produces<PagedResponse<CategoryResponse>>();
    }
}
