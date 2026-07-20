using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Pagination;
using Application.Products.Common;
using Application.Products.Common.Dtos;
using Application.Products.Mappers;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler(IApplicationDbContext context, ProductMapper mapper)
    : IQueryHandler<GetProductsQuery, PagedResponse<ProductResponse>>
{
    public async Task<Result<PagedResponse<ProductResponse>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Product> products = context.Products.AsNoTracking();

        ProductSort sort = query.Sort ?? ProductSort.Newest;
        int pageIndex = query.PageIndex ?? 1;
        int pageSize = query.PageSize ?? 20;

        products = products
            .ApplySearch(query.Search)
            .ApplyFilters(query, context)
            .ApplySorting(sort);

        int total = await products.CountAsync(cancellationToken);

        IQueryable<Product> page = products.ApplyPaging(pageIndex, pageSize);

        // Category + per-color photos are only needed by the shop grid's
        // interactive cards (color swap, hover slider) — the admin product
        // table and search typeahead share this same handler but never
        // render that data, so keep it opt-in rather than always paying for
        // the extra join/photo-URL presigning.
        //
        // Note: this two-level nested collection (Colors -> Photos) can
        // produce a cartesian-product row multiplication in the single SQL
        // query EF generates here. AsSplitQuery() would avoid that, but it
        // lives in Microsoft.EntityFrameworkCore.Relational, which this
        // (Application) project deliberately doesn't reference — keeping
        // the Application layer persistence-agnostic. Catalog sizes here
        // are modest (admin-curated), so accepting the single-query
        // behavior is the right trade-off over leaking a relational-only
        // dependency into this layer for one query.
        List<ProductDto> items = query.IncludeColors == true
            ? await page
                .Select(product => new ProductDto(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Variants.Min(v => v.Price),
                    product.Rating,
                    product.ProductColors
                        .SelectMany(pc => pc.Photos)
                        .OrderByDescending(p => p.IsMain)
                        .ThenBy(p => p.SortOrder)
                        .ThenBy(p => p.CreatedOnUtc)
                        .Select(p => p.FileName)
                        .FirstOrDefault(),
                    new CategoryDto(product.CategoryId, product.Category.Name),
                    product.Subcategory == null
                        ? null
                        : new SubcategoryDto(product.Subcategory.Id, product.Subcategory.Name),
                    // Raw inline LINQ rather than a shared extension method
                    // here — EF Core can't translate a custom IQueryable
                    // extension call made from this deep inside a nested
                    // Select projection.
                    context.Promotions
                        .Where(promo => promo.IsActive
                            && (promo.StartsAtUtc == null || promo.StartsAtUtc <= DateTime.UtcNow)
                            && (promo.EndsAtUtc == null || promo.EndsAtUtc >= DateTime.UtcNow)
                            && (promo.ProductId == product.Id || promo.BrandId == product.BrandId))
                        .OrderByDescending(promo => promo.DiscountPercentage)
                        .ThenBy(promo => promo.Id)
                        .Select(promo => (decimal?)promo.DiscountPercentage)
                        .FirstOrDefault(),
                    context.Promotions
                        .Where(promo => promo.IsActive
                            && (promo.StartsAtUtc == null || promo.StartsAtUtc <= DateTime.UtcNow)
                            && (promo.EndsAtUtc == null || promo.EndsAtUtc >= DateTime.UtcNow)
                            && (promo.ProductId == product.Id || promo.BrandId == product.BrandId))
                        .OrderByDescending(promo => promo.DiscountPercentage)
                        .ThenBy(promo => promo.Id)
                        .Select(promo => promo.EndsAtUtc)
                        .FirstOrDefault(),
                    product.ProductColors
                        .Select(pc => new ProductColorDto(
                            pc.Id,
                            pc.ColorId,
                            pc.Color.Name,
                            pc.Color.HexCode,
                            pc.Photos
                                .OrderByDescending(photo => photo.IsMain)
                                .ThenBy(photo => photo.SortOrder)
                                .ThenBy(photo => photo.CreatedOnUtc)
                                .Select(photo => new ProductPhotoDto(photo.Id, photo.FileName, photo.IsMain))
                                .ToList()))
                        .ToList()))
                .ToListAsync(cancellationToken)
            : await page
                .Select(product => new ProductDto(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Variants.Min(v => v.Price),
                    product.Rating,
                    product.ProductColors
                        .SelectMany(pc => pc.Photos)
                        .OrderByDescending(p => p.IsMain)
                        .ThenBy(p => p.SortOrder)
                        .ThenBy(p => p.CreatedOnUtc)
                        .Select(p => p.FileName)
                        .FirstOrDefault(),
                    null,
                    null,
                    null,
                    null,
                    null))
                .ToListAsync(cancellationToken);

        return new PagedResponse<ProductResponse>(
            items.Select(mapper.ToResponse).ToList(),
            pageIndex,
            pageSize,
            total);
    }
}
