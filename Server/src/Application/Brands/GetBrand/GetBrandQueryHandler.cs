using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.GetBrand;

internal sealed class GetBrandQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetBrandQuery, BrandResponse>
{
    private sealed record BrandRow(Guid Id, string Name, string? Description, string? LogoFileName, bool IsFeatured);

    public async Task<Result<BrandResponse>> Handle(GetBrandQuery query, CancellationToken cancellationToken)
    {
        BrandRow? brand = await context.Brands
            .AsNoTracking()
            .Where(b => b.Id == query.Id)
            .Select(b => new BrandRow(b.Id, b.Name, b.Description, b.LogoFileName, b.IsFeatured))
            .FirstOrDefaultAsync(cancellationToken);

        if (brand is null)
        {
            return Result.Failure<BrandResponse>(BrandErrors.NotFound(query.Id));
        }

        return new BrandResponse(
            brand.Id,
            brand.Name,
            brand.Description,
            brand.LogoFileName is null ? null : fileStorage.GetUrl(brand.LogoFileName).AbsoluteUri,
            brand.IsFeatured);
    }
}
