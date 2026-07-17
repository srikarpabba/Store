using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories.UpdateCategory;
using SharedKernel.Authorization;

namespace API.Endpoints.Categories;

internal sealed class UpdateCategory : IEndpoint
{
    public sealed record Request(string Name, string? Description, List<Guid> GenderIds);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateCategoryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCategoryCommand(id, request.Name, request.Description, request.GenderIds);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.CategoriesUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateCategory))
        .WithSummary("Updates a category's details and syncs its gender tags.")
        .WithDescription("Removing a gender tag deletes that gender's photo. At least one gender must remain tagged.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
