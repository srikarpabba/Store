using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories.ReorderCategories;
using SharedKernel.Authorization;

namespace API.Endpoints.Categories;

internal sealed class ReorderCategories : IEndpoint
{
    public sealed record Request(List<Guid> CategoryIds);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/genders/{genderId:guid}/order", async (
            Guid genderId,
            Request request,
            ICommandHandler<ReorderCategoriesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(
                new ReorderCategoriesCommand(genderId, request.CategoryIds),
                cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.CategoriesUpdate)
        .WithName(nameof(ReorderCategories))
        .WithSummary("Sets the storefront display order of a gender's categories.")
        .WithDescription("Send every category id tagged with the gender in the desired order. Each gender keeps its own order — /men and /women can differ.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
