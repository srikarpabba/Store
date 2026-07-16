using System.Diagnostics;
using Application.Abstractions.Home;
using Application.Common.Constants;
using Application.Home.GetHome;

namespace Infrastructure.Home;

internal sealed class HomeSectionService : IHomeSectionService
{

    public async Task<IReadOnlyList<HomeSectionResponse>> GetSectionsAsync(string storefront, CancellationToken cancellationToken)
    {
        storefront = storefront.Trim().ToUpperInvariant();

        return storefront switch
        {
            Storefronts.Men => await GetMenSectionsAsync(cancellationToken),
            Storefronts.Women => await GetWomenSectionsAsync(cancellationToken),
            Storefronts.Kids => await GetKidsSectionsAsync(cancellationToken),
            _ => throw new UnreachableException()
        };
    }

    private Task<IReadOnlyList<HomeSectionResponse>> GetMenSectionsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private Task<IReadOnlyList<HomeSectionResponse>> GetWomenSectionsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private Task<IReadOnlyList<HomeSectionResponse>> GetKidsSectionsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
