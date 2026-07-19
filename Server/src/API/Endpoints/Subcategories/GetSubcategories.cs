using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Subcategories;
using Application.Subcategories.GetSubcategories;

namespace API.Endpoints.Subcategories;

internal sealed class GetSubcategories : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetSubcategoriesQuery, IReadOnlyList<SubcategoryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetSubcategoriesQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetSubcategories))
        .WithSummary("Lists all subcategories with their parent category.")
        .Produces<IReadOnlyList<SubcategoryResponse>>();
    }
}
