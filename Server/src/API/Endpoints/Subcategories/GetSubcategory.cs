using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Subcategories;
using Application.Subcategories.GetSubcategory;

namespace API.Endpoints.Subcategories;

internal sealed class GetSubcategory : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetSubcategoryQuery, SubcategoryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetSubcategoryQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetSubcategory))
        .WithSummary("Gets a single subcategory.")
        .Produces<SubcategoryResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
