using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories;
using Application.Categories.GetCategory;

namespace API.Endpoints.Categories;

internal sealed class GetCategory : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetCategoryQuery, CategoryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetCategoryQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetCategory))
        .WithSummary("Gets a single category with its gender tags and photos.")
        .Produces<CategoryResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
