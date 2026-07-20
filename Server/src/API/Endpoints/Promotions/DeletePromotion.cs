using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Promotions.DeletePromotion;
using SharedKernel.Authorization;

namespace API.Endpoints.Promotions;

internal sealed class DeletePromotion : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeletePromotionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeletePromotionCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.PromotionsDelete)
        .WithName(nameof(DeletePromotion))
        .WithSummary("Deletes a promotion.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
