using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Subcategories.DeleteSubcategory;
using SharedKernel.Authorization;

namespace API.Endpoints.Subcategories;

internal sealed class DeleteSubcategory : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteSubcategoryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteSubcategoryCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.SubcategoriesDelete)
        .WithName(nameof(DeleteSubcategory))
        .WithSummary("Deletes a subcategory.")
        .WithDescription("Blocked with a conflict if any product still uses this subcategory.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
