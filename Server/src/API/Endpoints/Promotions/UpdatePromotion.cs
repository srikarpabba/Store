using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Promotions.UpdatePromotion;
using SharedKernel.Authorization;

namespace API.Endpoints.Promotions;

internal sealed class UpdatePromotion : IEndpoint
{
    public sealed record Request(
        string Name,
        decimal DiscountPercentage,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        bool IsActive,
        Guid? ProductId,
        Guid? BrandId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdatePromotionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePromotionCommand(
                id,
                request.Name,
                request.DiscountPercentage,
                request.StartsAtUtc,
                request.EndsAtUtc,
                request.IsActive,
                request.ProductId,
                request.BrandId);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.PromotionsUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdatePromotion))
        .WithSummary("Updates a promotion's discount, schedule and scope.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
