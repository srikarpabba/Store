using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.GetProfile;

namespace API.Endpoints.Users;

internal sealed class GetMyProfile : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            IQueryHandler<GetMyProfileQuery, ProfileResponse> handler,
            CancellationToken cancellationToken) =>
        {
            SharedKernel.Result<ProfileResponse> result = await handler.Handle(new GetMyProfileQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
