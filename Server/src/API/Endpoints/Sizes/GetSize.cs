using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Sizes;
using Application.Sizes.GetSize;

namespace API.Endpoints.Sizes;

internal sealed class GetSize : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetSizeQuery, SizeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new GetSizeQuery(id), cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetSize))
        .WithSummary("Gets a single size.")
        .Produces<SizeResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
