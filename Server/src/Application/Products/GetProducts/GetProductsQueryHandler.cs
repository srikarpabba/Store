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
            .ApplyFilters(query)
            .ApplySorting(sort);

        int total = await products.CountAsync(cancellationToken);

        List<ProductDto> items = await products
            .ApplyPaging(pageIndex, pageSize)
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Variants.Min(v => v.Price),
                product.Rating,
                product.ProductColors
                    .SelectMany(pc => pc.Photos)
                    .OrderByDescending(p => p.IsMain)
                    .ThenBy(p => p.CreatedOnUtc)
                    .Select(p => p.FileName)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductResponse>(
            items.Select(mapper.ToResponse).ToList(),
            pageIndex,
            pageSize,
            total);
    }
}
