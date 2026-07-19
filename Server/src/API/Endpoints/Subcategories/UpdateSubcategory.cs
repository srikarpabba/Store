using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Subcategories.UpdateSubcategory;
using SharedKernel.Authorization;

namespace API.Endpoints.Subcategories;

internal sealed class UpdateSubcategory : IEndpoint
{
    public sealed record Request(string Name, Guid CategoryId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSubcategoryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSubcategoryCommand(id, request.Name, request.CategoryId);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.SubcategoriesUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateSubcategory))
        .WithSummary("Updates a subcategory's name or parent category.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
