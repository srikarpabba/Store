using Application.Storefront.GetStorefrontSections;

namespace Application.Abstractions.Storefront;

public interface IStorefrontSectionService
{
    Task<IReadOnlyList<StorefrontSectionResponse>> GetSectionsAsync(
        string storefront,
        CancellationToken cancellationToken);
}
