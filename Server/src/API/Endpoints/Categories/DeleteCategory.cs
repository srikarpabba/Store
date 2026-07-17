using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories.DeleteCategory;
using SharedKernel.Authorization;

namespace API.Endpoints.Categories;

internal sealed class DeleteCategory : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteCategoryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteCategoryCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.CategoriesDelete)
        .WithName(nameof(DeleteCategory))
        .WithSummary("Deletes a category.")
        .WithDescription("Blocked with a conflict if any product still uses this category.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
