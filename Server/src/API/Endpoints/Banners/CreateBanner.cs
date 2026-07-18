using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Banners.CreateBanner;
using SharedKernel.Authorization;

namespace API.Endpoints.Banners;

internal sealed class CreateBanner : IEndpoint
{
    public sealed record Request(string Storefront, string? Title, string? Link, int SortOrder, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateBannerCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBannerCommand(
                request.Storefront,
                request.Title,
                request.Link,
                request.SortOrder,
                request.IsActive);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetBanner), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.BannersCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateBanner))
        .WithSummary("Creates a banner for a storefront page.")
        .WithDescription("The image is uploaded separately after creation via /banners/{id}/image.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
