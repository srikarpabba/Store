using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Common.Pagination;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.GetBrands;

internal sealed class GetBrandsQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetBrandsQuery, PagedResponse<BrandResponse>>
{
    private const int DefaultPageSize = 25;

    private sealed record BrandRow(Guid Id, string Name, string? Description, string? LogoFileName, bool IsFeatured);

    public async Task<Result<PagedResponse<BrandResponse>>> Handle(
        GetBrandsQuery query,
        CancellationToken cancellationToken)
    {
        int pageIndex = query.PageIndex ?? 1;
        int pageSize = query.PageSize ?? DefaultPageSize;

        IQueryable<Brand> brands = context.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name);

        int total = await brands.CountAsync(cancellationToken);

        List<BrandRow> rows = await brands
            .ApplyPaging(pageIndex, pageSize)
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

        return new PagedResponse<BrandResponse>(items, pageIndex, pageSize, total);
    }
}
