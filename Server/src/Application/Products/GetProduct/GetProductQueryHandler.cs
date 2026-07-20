using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products.Common.Dtos;
using Application.Products.Mappers;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.GetProduct;

internal sealed class GetProductQueryHandler(IApplicationDbContext context, ProductMapper mapper)
    : IQueryHandler<GetProductQuery, ProductDetailsResponse>
{
    public async Task<Result<ProductDetailsResponse>> Handle(
        GetProductQuery query,
        CancellationToken cancellationToken)
    {
        ProductDetailsDto? dto = await context.Products
            .AsNoTracking()
            .Where(product => product.Id == query.ProductId)
            .Select(product => new ProductDetailsDto(
                product.Id,
                product.Name,
                product.Description,
                new CategoryDto(
                    product.CategoryId,
                    product.Category.Name),
                product.Subcategory == null
                    ? null
                    : new SubcategoryDto(product.Subcategory.Id, product.Subcategory.Name),
                new BrandDto(
                    product.BrandId,
                    product.Brand.Name),
                product.Rating,
                // Raw inline LINQ rather than a shared extension method here —
                // EF Core can't translate a custom IQueryable extension call
                // made from this deep inside a nested Select projection.
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
                    .Select(color => new ProductColorDto(
                        color.Id,
                        color.ColorId,
                        color.Color.Name,
                        color.Color.HexCode,

                        color.Photos
                            .OrderByDescending(p => p.IsMain)
                            .ThenBy(p => p.SortOrder)
                            .ThenBy(p => p.CreatedOnUtc)
                            .Select(photo => new ProductPhotoDto(
                                photo.Id,
                                photo.FileName,
                                photo.IsMain))
                            .ToList()))
                    .ToList(),

                product.ProductGenders
                    .Select(g => new GenderDto(
                        g.GenderId,
                        g.Gender.Name))
                    .ToList(),

                product.Variants
                    .Select(v => new ProductVariantDto(
                        v.Id,
                        v.ProductColorId,
                        v.SizeId,
                        v.Size.Name,
                        v.Price,
                        v.QuantityInStock,
                        v.SKU))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            return Result.Failure<ProductDetailsResponse>(
                ProductErrors.NotFound(query.ProductId));
        }

        return mapper.ToResponse(dto);
    }
}
