using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Colors;
using Application.Colors.GetColor;

namespace API.Endpoints.Colors;

internal sealed class GetColor : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetColorQuery, ColorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetColorQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetColor))
        .WithSummary("Gets a single color.")
        .Produces<ColorResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
