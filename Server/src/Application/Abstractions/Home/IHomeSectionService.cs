using Application.Home.GetHome;

namespace Application.Abstractions.Home;

public interface IHomeSectionService
{
    Task<IReadOnlyList<HomeSectionResponse>> GetSectionsAsync(
        string storefront,
        CancellationToken cancellationToken);
}
