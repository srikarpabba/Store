using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IApplicationDbContext context,
    ProductValidator validator)
    : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Product? product = await context.Products
            .Include(p => p.ProductGenders)
            .Include(p => p.ProductColors).ThenInclude(c => c.Photos)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(command.Id));
        }

        Result result = await ValidateProductDetailsAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        product.Update(
            command.Name,
            command.Description,
            command.CategoryId,
            command.BrandId,
            command.SubcategoryId);

        SyncGenders(product, command);

        Result variantsResult = SyncVariants(product, command);

        if (variantsResult.IsFailure)
        {
            return variantsResult;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static void SyncGenders(Product product, UpdateProductCommand command)
    {
        foreach (ProductGender gender in product.ProductGenders
            .Where(g => !command.GenderIds.Contains(g.GenderId))
            .ToList())
        {
            product.RemoveGender(gender);
        }

        foreach (Guid genderId in command.GenderIds
            .Where(id => !product.ProductGenders.Any(g => g.GenderId == id)))
        {
            product.AddGender(genderId);
        }
    }

    // New/removed children are added to and removed from the context
    // explicitly: entity ids are assigned at construction while the columns
    // are ValueGeneratedOnAdd, so entities merely discovered via navigation
    // fixup would be tracked as Modified (their key "already has a value")
    // and SaveChanges would UPDATE rows that do not exist
    private Result SyncVariants(Product product, UpdateProductCommand command)
    {
        var keptVariantIds = command.Variants
            .Where(v => v.Id is not null)
            .Select(v => v.Id!.Value)
            .ToHashSet();

        foreach (ProductVariant variant in product.Variants
            .Where(v => !keptVariantIds.Contains(v.Id))
            .ToList())
        {
            product.RemoveVariant(variant);

            // Product.Variants is an optional relationship — removing from
            // the collection alone would only null the FK and orphan the row
            context.ProductVariants.Remove(variant);
        }

        var colors = product.ProductColors
            .ToDictionary(c => c.ColorId);

        foreach (UpdateVariantRequest request in command.Variants)
        {
            if (!colors.TryGetValue(request.ColorId, out ProductColor? color))
            {
                color = product.AddColor(request.ColorId);
                context.ProductColors.Add(color);
                colors.Add(request.ColorId, color);
            }

            if (request.Id is { } variantId)
            {
                ProductVariant? variant = product.Variants
                    .FirstOrDefault(v => v.Id == variantId);

                if (variant is null)
                {
                    return Result.Failure(ProductErrors.VariantNotFound(variantId));
                }

                variant.Update(color, request.SizeId, request.Price, request.QuantityInStock, request.SKU);
            }
            else
            {
                ProductVariant variant = product.AddVariant(
                    color,
                    request.SizeId,
                    request.Price,
                    request.QuantityInStock,
                    request.SKU);

                context.ProductVariants.Add(variant);
            }
        }

        // Drop colors that no longer back any variant — unless photos are
        // attached, which would be silently lost with the color
        foreach (ProductColor color in product.ProductColors
            .Where(c => c.Photos.Count == 0
                && !product.Variants.Any(v => v.ProductColorId == c.Id))
            .ToList())
        {
            product.RemoveColor(color);
            context.ProductColors.Remove(color);
        }

        return Result.Success();
    }

    private async Task<Result> ValidateProductDetailsAsync(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Result result = await validator.ValidateCategoryAsync(command.CategoryId, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        result = await validator.ValidateSubcategoryAsync(command.SubcategoryId, command.CategoryId, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        result = await validator.ValidateBrandAsync(command.BrandId, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        result = await validator.ValidateGenderIdsAsync(command.GenderIds, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        result = await validator.ValidateCategoryGenderCompatibilityAsync(
            command.CategoryId,
            command.GenderIds,
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        result = await validator.ValidateColorIdsAsync(
            command.Variants.Select(x => x.ColorId).Distinct().ToList(),
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        result = await validator.ValidateSizeIdsAsync(
            command.Variants.Select(x => x.SizeId).Distinct().ToList(),
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        // A product's own SKUs must not block its update
        return await validator.ValidateSkusAsync(
            command.Variants.Select(x => x.SKU).ToList(),
            cancellationToken,
            excludeProductId: command.Id);
    }
}
