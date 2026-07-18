using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Storefront.GetStorefrontSections;

namespace API.Endpoints.Storefronts;

internal sealed class GetStorefrontSections : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{storefront}", async (
            [AsParameters] GetStorefrontSectionsQuery query,
            IQueryHandler<GetStorefrontSectionsQuery, StorefrontSectionsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(query, cancellationToken))
                .Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetStorefrontSections))
        .WithSummary("Gets the banner, category and other sections for a storefront page.")
        .WithDescription("Storefront is one of \"men\", \"women\" or \"kids\" — this drives the Men/Women shop landing pages, not the app's home page.")
        .WithRequestValidation<GetStorefrontSectionsQuery>()
        .Produces<StorefrontSectionsResponse>();
    }
}
