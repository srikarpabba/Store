using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Categories.DeleteCategoryGenderPhoto;
using SharedKernel.Authorization;

namespace API.Endpoints.Categories;

internal sealed class DeleteCategoryGenderPhoto : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{categoryId:guid}/genders/{genderId:guid}/photo", async (
            Guid categoryId,
            Guid genderId,
            ICommandHandler<DeleteCategoryGenderPhotoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteCategoryGenderPhotoCommand(categoryId, genderId), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.CategoriesUpdate)
        .WithName(nameof(DeleteCategoryGenderPhoto))
        .WithSummary("Deletes the display photo for a category's gender tag.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
