using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Home.GetHome;

namespace API.Endpoints.Home;

internal sealed class GetHome : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetHomeQuery query,
            IQueryHandler<GetHomeQuery, HomeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(query, cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetHome")
        .WithSummary("Gets the home page for a storefront")
        .WithRequestValidation<GetHomeQuery>();
    }
}
