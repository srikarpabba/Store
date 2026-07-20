using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Promotions.CreatePromotionBatch;
using SharedKernel.Authorization;

namespace API.Endpoints.Promotions;

internal sealed class CreatePromotionBatch : IEndpoint
{
    public sealed record RequestItem(
        decimal DiscountPercentage,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        bool IsActive,
        Guid? ProductId,
        Guid? BrandId);

    public sealed record Request(string Name, List<RequestItem> Items);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/batch", async (
            Request request,
            ICommandHandler<CreatePromotionBatchCommand, IReadOnlyList<Guid>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePromotionBatchCommand(
                request.Name,
                request.Items
                    .Select(i => new CreatePromotionBatchItem(
                        i.DiscountPercentage,
                        i.StartsAtUtc,
                        i.EndsAtUtc,
                        i.IsActive,
                        i.ProductId,
                        i.BrandId))
                    .ToList());

            SharedKernel.Result<IReadOnlyList<Guid>> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.PromotionsCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreatePromotionBatch))
        .WithSummary("Creates several promotions at once under a shared sale name.")
        .WithDescription("Each item is scoped to exactly one of a product or a brand, with its own discount and optional schedule. All items succeed or fail together.")
        .Produces<IReadOnlyList<Guid>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
