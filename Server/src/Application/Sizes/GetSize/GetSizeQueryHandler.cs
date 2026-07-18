using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Sizes.GetSize;

internal sealed class GetSizeQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSizeQuery, SizeResponse>
{
    public async Task<Result<SizeResponse>> Handle(GetSizeQuery query, CancellationToken cancellationToken)
    {
        SizeResponse? size = await context.Sizes
            .AsNoTracking()
            .Where(s => s.Id == query.Id)
            .Select(s => new SizeResponse(s.Id, s.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (size is null)
        {
            return Result.Failure<SizeResponse>(SizeErrors.NotFound(query.Id));
        }

        return size;
    }
}
