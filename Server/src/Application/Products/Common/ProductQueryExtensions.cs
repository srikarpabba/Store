using Application.Products.GetProducts;
using Domain.Products;

namespace Application.Products.Common;

internal static class ProductQueryExtensions
{
    public static IQueryable<Product> ApplySearch(
        this IQueryable<Product> query,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        // Lowercasing both sides makes the match case-insensitive: EF translates
        // ToLower()/Contains() to SQL LOWER(...) LIKE, which runs in Postgres.
        // The culture/StringComparison overloads the analyzers suggest neither
        // apply to server-side SQL nor translate to it, hence the suppressions.
#pragma warning disable CA1304, CA1308, CA1311, CA1862
        string pattern = search.Trim().ToLowerInvariant();

        return query.Where(x =>
                x.Name.ToLower().Contains(pattern) ||
                x.Description.ToLower().Contains(pattern) ||
                x.Brand.Name.ToLower().Contains(pattern) ||
                x.Category.Name.ToLower().Contains(pattern));
#pragma warning restore CA1304, CA1308, CA1311, CA1862
    }

    public static IQueryable<Product> ApplyFilters(
        this IQueryable<Product> query,
        GetProductsQuery filters)
    {
        if (filters.Brands is { Length: > 0 })
        {
            query = query.Where(x =>
                filters.Brands.Contains(x.Brand.Name));
        }

        if (filters.Categories is { Length: > 0 })
        {
            query = query.Where(x =>
                filters.Categories.Contains(x.Category.Name));
        }

        if (filters.Subcategories is { Length: > 0 })
        {
            query = query.Where(x =>
                x.Subcategory != null && filters.Subcategories.Contains(x.Subcategory.Name));
        }

        if (filters.Colors is { Length: > 0 })
        {
            query = query.Where(x =>
                x.ProductColors.Any(pc =>
                    filters.Colors.Contains(pc.Color.Name)));
        }

        if (filters.Sizes is { Length: > 0 })
        {
            query = query.Where(x =>
                x.Variants.Any(v =>
                    filters.Sizes.Contains(v.Size.Name)));
        }

        if (filters.Genders is { Length: > 0 })
        {
            query = query.Where(x =>
                x.ProductGenders.Any(pg =>
                    filters.Genders.Contains(pg.Gender.Name)));
        }

        if (filters.MinPrice.HasValue)
        {
            query = query.Where(x =>
                x.Variants.Min(v => v.Price) >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            query = query.Where(x =>
                x.Variants.Min(v => v.Price) <= filters.MaxPrice.Value);
        }

        return query;
    }

    public static IQueryable<Product> ApplySorting(
            this IQueryable<Product> query,
            ProductSort sort)
    {
        return sort switch
        {
            ProductSort.Name =>
                query.OrderBy(x => x.Name),

            ProductSort.PriceLowToHigh =>
                query.OrderBy(x => x.Variants.Min(v => v.Price)),

            ProductSort.PriceHighToLow =>
                query.OrderByDescending(x => x.Variants.Min(v => v.Price)),

            ProductSort.Rating =>
                query.OrderByDescending(x => x.Rating),

            ProductSort.Newest =>
                query.OrderByDescending(x => x.CreatedOnUtc),

            _ =>
                query.OrderBy(x => x.Name)
        };
    }

    public static IQueryable<Product> ApplyPaging(
        this IQueryable<Product> query,
        int page,
        int pageSize)
    {
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}
