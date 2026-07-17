using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Brands.UploadBrandLogo;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization;

namespace API.Endpoints.Brands;

internal sealed class UploadBrandLogo : IEndpoint
{
    public sealed record Request(IFormFile File);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/logo", async (
            Guid id,
            [FromForm] Request request,
            ICommandHandler<UploadBrandLogoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var upload = new FileUpload(
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                request.File.OpenReadStream());

            var command = new UploadBrandLogoCommand(id, upload);

            return (await handler.Handle(command, cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .DisableAntiforgery()
        .HasPermission(Permissions.BrandsUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UploadBrandLogo))
        .WithSummary("Uploads a brand's logo.")
        .WithDescription("Replaces any existing logo. Accepts JPEG, PNG or WebP up to 5 MB.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
