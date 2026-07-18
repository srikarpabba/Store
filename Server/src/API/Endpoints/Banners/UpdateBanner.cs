using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Banners.UpdateBanner;
using SharedKernel.Authorization;

namespace API.Endpoints.Banners;

internal sealed class UpdateBanner : IEndpoint
{
    public sealed record Request(string Storefront, string? Title, string? Link, int SortOrder, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateBannerCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateBannerCommand(
                id,
                request.Storefront,
                request.Title,
                request.Link,
                request.SortOrder,
                request.IsActive);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.BannersUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateBanner))
        .WithSummary("Updates a banner's details.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
