using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Sizes;
using Application.Sizes.GetSizes;

namespace API.Endpoints.Sizes;

internal sealed class GetSizes : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetSizesQuery, IReadOnlyList<SizeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetSizesQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetSizes))
        .WithSummary("Lists all sizes.")
        .Produces<IReadOnlyList<SizeResponse>>();
    }
}
