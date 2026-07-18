using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Sizes.GetSizes;

internal sealed class GetSizesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSizesQuery, IReadOnlyList<SizeResponse>>
{
    public async Task<Result<IReadOnlyList<SizeResponse>>> Handle(
        GetSizesQuery query,
        CancellationToken cancellationToken)
    {
        List<SizeResponse> sizes = await context.Sizes
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SizeResponse(s.Id, s.Name))
            .ToListAsync(cancellationToken);

        return sizes;
    }
}
