using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories.CreateCategory;
using SharedKernel.Authorization;

namespace API.Endpoints.Categories;

internal sealed class CreateCategory : IEndpoint
{
    public sealed record Request(string Name, string? Description, List<Guid> GenderIds, List<Guid>? SizeIds);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateCategoryCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCategoryCommand(request.Name, request.Description, request.GenderIds, request.SizeIds ?? []);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetCategory), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.CategoriesCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateCategory))
        .WithSummary("Creates a category tagged with one or more genders.")
        .WithDescription("A category must be tagged with at least one gender to be usable by a product — there is no unisex default. Photos are uploaded separately per gender via /categories/{id}/genders/{genderId}/photo.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
