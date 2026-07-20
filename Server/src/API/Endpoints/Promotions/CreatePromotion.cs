using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Promotions.CreatePromotion;
using SharedKernel.Authorization;

namespace API.Endpoints.Promotions;

internal sealed class CreatePromotion : IEndpoint
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
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreatePromotionCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePromotionCommand(
                request.Name,
                request.DiscountPercentage,
                request.StartsAtUtc,
                request.EndsAtUtc,
                request.IsActive,
                request.ProductId,
                request.BrandId);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetPromotion), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.PromotionsCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreatePromotion))
        .WithSummary("Creates a percentage-off sale for a product or a brand.")
        .WithDescription("Scoped to exactly one of ProductId or BrandId. When active promotions overlap for the same product (its own plus its brand's), the highest discount wins for display.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
