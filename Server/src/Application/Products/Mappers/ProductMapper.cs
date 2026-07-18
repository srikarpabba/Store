using Application.Abstractions.Storage;
using Application.Products.Common.Dtos;
using Application.Products.Common.Responses;
using Application.Products.GetProduct;
using Application.Products.GetProducts;

namespace Application.Products.Mappers;

internal sealed class ProductMapper(IFileStorage fileStorage)
{
    public ProductResponse ToResponse(ProductDto dto)
    {
        return new ProductResponse(
            dto.Id,
            dto.Name,
            dto.StartingPrice,
            dto.Rating,
            dto.Image is null
                ? null
                : fileStorage.GetUrl(dto.Image).AbsoluteUri,
            dto.Category is null
                ? null
                : new ProductCategoryResponse(dto.Category.Id, dto.Category.Name),
            dto.Colors?
                .Select(color => new ProductColorResponse(
                    color.Id,
                    color.ColorId,
                    color.Name,
                    color.HexCode,

                    color.Photos
                        .Select(photo => new ProductPhotoResponse(
                            photo.Id,
                            fileStorage.GetUrl(photo.FileName).AbsoluteUri,
                            photo.IsMain))
                        .ToList()))
                .ToList());
    }

    public ProductDetailsResponse ToResponse(ProductDetailsDto dto)
    {
        return new ProductDetailsResponse(
            dto.Id,
            dto.Name,
            dto.Description,
            new ProductCategoryResponse(
                dto.Category.Id,
                dto.Category.Name),
            new BrandResponse(
                dto.Brand.Id,
                dto.Brand.Name),
                dto.Rating,

            dto.Colors
                .Select(color => new ProductColorResponse(
                    color.Id,
                    color.ColorId,
                    color.Name,
                    color.HexCode,

                    color.Photos
                        .Select(photo => new ProductPhotoResponse(
                            photo.Id,
                            fileStorage.GetUrl(photo.FileName).AbsoluteUri,
                            photo.IsMain))
                        .ToList()))
                .ToList(),

            dto.Genders
                .Select(g => new GenderResponse(
                    g.Id,
                    g.Name))
                .ToList(),

            dto.Variants
                .Select(variant => new ProductVariantResponse(
                    variant.Id,
                    variant.ProductColorId,
                    variant.SizeId,
                    variant.Size,
                    variant.Price,
                    variant.QuantityInStock,
                    variant.SKU))
                .ToList());
    }
}
