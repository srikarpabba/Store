using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories;
using Application.Categories.GetCategories;

namespace API.Endpoints.Categories;

internal sealed class GetCategories : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetCategoriesQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetCategories))
        .WithSummary("Lists all categories with their gender tags and photos.")
        .Produces<IReadOnlyList<CategoryResponse>>();
    }
}
