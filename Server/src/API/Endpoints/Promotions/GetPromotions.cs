using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Promotions;
using Application.Promotions.GetPromotions;
using SharedKernel.Authorization;

namespace API.Endpoints.Promotions;

internal sealed class GetPromotions : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetPromotionsQuery, IReadOnlyList<PromotionResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetPromotionsQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.PromotionsRead)
        .WithName(nameof(GetPromotions))
        .WithSummary("Lists promotions for the admin dashboard.")
        .WithDescription("Admin-only — storefront prices already reflect any active discount, computed server-side.")
        .Produces<IReadOnlyList<PromotionResponse>>();
    }
}
