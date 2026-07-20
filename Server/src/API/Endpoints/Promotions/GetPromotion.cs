using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Promotions;
using Application.Promotions.GetPromotion;
using SharedKernel.Authorization;

namespace API.Endpoints.Promotions;

internal sealed class GetPromotion : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetPromotionQuery, PromotionResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetPromotionQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.PromotionsRead)
        .WithName(nameof(GetPromotion))
        .WithSummary("Gets a single promotion for editing.")
        .Produces<PromotionResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
