using Application.Abstractions.Messaging;
using Application.Abstractions.Storefront;
using SharedKernel;

namespace Application.Storefront.GetStorefrontSections;

internal sealed class GetStorefrontSectionsQueryHandler(
    IStorefrontSectionService storefrontSectionService)
    : IQueryHandler<GetStorefrontSectionsQuery, StorefrontSectionsResponse>
{
    public async Task<Result<StorefrontSectionsResponse>> Handle(
        GetStorefrontSectionsQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<StorefrontSectionResponse> sections =
            await storefrontSectionService.GetSectionsAsync(
                query.Storefront,
                cancellationToken);

        return new StorefrontSectionsResponse(sections);
    }
}
