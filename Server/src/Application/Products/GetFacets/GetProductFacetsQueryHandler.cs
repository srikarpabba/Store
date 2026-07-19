using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products.Common;
using Application.Products.GetProducts;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.GetFacets;

internal sealed class GetProductFacetsQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetProductFacetsQuery, ProductFacetsResponse>
{
    public async Task<Result<ProductFacetsResponse>> Handle(
        GetProductFacetsQuery query,
        CancellationToken cancellationToken)
    {
        var filters = new GetProductsQuery(
            query.Search,
            query.Brands,
            query.Categories,
            query.Subcategories,
            query.Colors,
            query.Sizes,
            query.Genders,
            query.MinPrice,
            query.MaxPrice,
            Sort: null,
            PageIndex: null,
            PageSize: null,
            IncludeColors: null);

        List<FacetCount> subcategories = await CountAsync(
            Filtered(filters with { Subcategories = null })
                .Where(p => p.Subcategory != null)
                .Select(p => p.Subcategory!.Name),
            cancellationToken);

        List<FacetCount> brands = await CountAsync(
            Filtered(filters with { Brands = null })
                .Select(p => p.Brand.Name),
            cancellationToken);

        List<FacetCount> colors = await CountAsync(
            Filtered(filters with { Colors = null })
                .SelectMany(p => p.ProductColors.Select(pc => pc.Color.Name)),
            cancellationToken);

        // Distinct per product: size M in two colors is still one product
        List<FacetCount> sizes = await CountAsync(
            Filtered(filters with { Sizes = null })
                .SelectMany(p => p.Variants.Select(v => v.Size.Name).Distinct()),
            cancellationToken);

        return new ProductFacetsResponse(subcategories, brands, colors, sizes);
    }

    private IQueryable<Product> Filtered(GetProductsQuery filters)
    {
        return context.Products
            .AsNoTracking()
            .ApplySearch(filters.Search)
            .ApplyFilters(filters);
    }

    /// <summary>
    /// Groups the projected names server-side, but shapes into
    /// <see cref="FacetCount"/> in memory — EF can't translate constructor
    /// calls inside a grouped projection.
    /// </summary>
    private static async Task<List<FacetCount>> CountAsync(
        IQueryable<string> names,
        CancellationToken cancellationToken)
    {
        var rows = await names
            .GroupBy(name => name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return rows.Select(x => new FacetCount(x.Name, x.Count)).ToList();
    }
}
