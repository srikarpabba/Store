using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Subcategories.CreateSubcategory;
using SharedKernel.Authorization;

namespace API.Endpoints.Subcategories;

internal sealed class CreateSubcategory : IEndpoint
{
    public sealed record Request(string Name, Guid CategoryId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateSubcategoryCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateSubcategoryCommand(request.Name, request.CategoryId);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetSubcategory), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.SubcategoriesCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateSubcategory))
        .WithSummary("Creates a subcategory under a category.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
