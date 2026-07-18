using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Banners.UploadBannerImage;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization;

namespace API.Endpoints.Banners;

internal sealed class UploadBannerImage : IEndpoint
{
    public sealed record Request(IFormFile File);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/image", async (
            Guid id,
            [FromForm] Request request,
            ICommandHandler<UploadBannerImageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var upload = new FileUpload(
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                request.File.OpenReadStream());

            var command = new UploadBannerImageCommand(id, upload);

            return (await handler.Handle(command, cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .DisableAntiforgery()
        .HasPermission(Permissions.BannersUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UploadBannerImage))
        .WithSummary("Uploads a banner's image.")
        .WithDescription("Replaces any existing image. Accepts JPEG, PNG or WebP up to 5 MB.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
