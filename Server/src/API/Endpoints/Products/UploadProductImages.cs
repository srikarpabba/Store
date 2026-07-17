using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Products.UploadProductImages;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class UploadProductImages : IEndpoint
{
    public sealed record Request(Guid ProductColorId, IReadOnlyList<IFormFile> Files);
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{productId:guid}/images", async (
            Guid productId,
            [FromForm] Request request,
            ICommandHandler<UploadProductImagesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var uploads = request.Files
                .Select(file => new FileUpload(
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    file.OpenReadStream()))
                .ToList();

            var command = new UploadProductImagesCommand(
                productId,
                request.ProductColorId,
                uploads);

            return (await handler.Handle(command, cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .DisableAntiforgery()
        .HasPermission(Permissions.ProductsCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UploadProductImages))
        .WithSummary("Uploads one or more photos for a product color.")
        .WithDescription("The first photo uploaded for a color becomes its main photo. Accepts JPEG, PNG or WebP up to 5 MB each.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
