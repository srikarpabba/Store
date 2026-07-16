using Application.Abstractions.Home;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Home.GetHome;

internal sealed class GetHomeQueryHandler(
    IHomeSectionService homeSectionService)
    : IQueryHandler<GetHomeQuery, HomeResponse>
{
    public async Task<Result<HomeResponse>> Handle(
        GetHomeQuery query,
        CancellationToken cancellationToken)
    {

        IReadOnlyList<HomeSectionResponse> sections =
            await homeSectionService.GetSectionsAsync(
                query.Storefront,
                cancellationToken);

        return new HomeResponse(sections);
    }
}
