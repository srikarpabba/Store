using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Banners.DeleteBanner;
using SharedKernel.Authorization;

namespace API.Endpoints.Banners;

internal sealed class DeleteBanner : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteBannerCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteBannerCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.BannersDelete)
        .WithName(nameof(DeleteBanner))
        .WithSummary("Deletes a banner.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
