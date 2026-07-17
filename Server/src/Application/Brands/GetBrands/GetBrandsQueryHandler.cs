using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.GetBrands;

internal sealed class GetBrandsQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetBrandsQuery, IReadOnlyList<BrandResponse>>
{
    private sealed record BrandRow(Guid Id, string Name, string? Description, string? LogoFileName, bool IsFeatured);

    public async Task<Result<IReadOnlyList<BrandResponse>>> Handle(
        GetBrandsQuery query,
        CancellationToken cancellationToken)
    {
        List<BrandRow> rows = await context.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandRow(b.Id, b.Name, b.Description, b.LogoFileName, b.IsFeatured))
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new BrandResponse(
                r.Id,
                r.Name,
                r.Description,
                r.LogoFileName is null ? null : fileStorage.GetUrl(r.LogoFileName).AbsoluteUri,
                r.IsFeatured))
            .ToList();

        return items;
    }
}
