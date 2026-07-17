using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Categories.UploadCategoryGenderPhoto;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization;

namespace API.Endpoints.Categories;

internal sealed class UploadCategoryGenderPhoto : IEndpoint
{
    public sealed record Request(IFormFile File);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{categoryId:guid}/genders/{genderId:guid}/photo", async (
            Guid categoryId,
            Guid genderId,
            [FromForm] Request request,
            ICommandHandler<UploadCategoryGenderPhotoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var upload = new FileUpload(
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                request.File.OpenReadStream());

            var command = new UploadCategoryGenderPhotoCommand(categoryId, genderId, upload);

            return (await handler.Handle(command, cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .DisableAntiforgery()
        .HasPermission(Permissions.CategoriesUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UploadCategoryGenderPhoto))
        .WithSummary("Uploads the display photo for a category's gender tag.")
        .WithDescription("Replaces any existing photo for that category/gender pair. Accepts JPEG, PNG or WebP up to 5 MB.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
