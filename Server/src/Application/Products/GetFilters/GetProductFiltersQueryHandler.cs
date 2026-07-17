using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.GetFilters;

internal sealed class GetProductFiltersQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetProductFiltersQuery, ProductFiltersResponse>
{
    public async Task<Result<ProductFiltersResponse>> Handle(
        GetProductFiltersQuery query,
        CancellationToken cancellationToken)
    {
        List<LookupResponse> brands = await context.Brands
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        List<LookupResponse> categories = await context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        List<LookupResponse> colors = await context.Colors
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        List<LookupResponse> sizes = await context.Sizes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        List<LookupResponse> genders = await context.Genders
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LookupResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        bool hasVariants = await context.ProductVariants
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        decimal minPrice = 0m;
        decimal maxPrice = 0m;

        if (hasVariants)
        {
            minPrice = await context.ProductVariants.AsNoTracking().MinAsync(x => x.Price, cancellationToken);
            maxPrice = await context.ProductVariants.AsNoTracking().MaxAsync(x => x.Price, cancellationToken);
        }

        return new ProductFiltersResponse(
            brands,
            categories,
            colors,
            sizes,
            genders,
            minPrice,
            maxPrice);
    }
}
