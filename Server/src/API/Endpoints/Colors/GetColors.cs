using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Colors;
using Application.Colors.GetColors;

namespace API.Endpoints.Colors;

internal sealed class GetColors : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetColorsQuery, IReadOnlyList<ColorResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetColorsQuery(), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetColors))
        .WithSummary("Lists all colors.")
        .Produces<IReadOnlyList<ColorResponse>>();
    }
}
