using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using SharedKernel;

namespace Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IApplicationDbContext context,
    ProductValidator validator)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Result result = await ValidateProductDetailsAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        var product = Product.Create(
            command.Name,
            command.Description,
            command.CategoryId,
            command.BrandId);

        foreach (Guid genderId in command.GenderIds)
        {
            product.AddGender(genderId);
        }

        Dictionary<Guid, ProductColor> colors = [];

        foreach (Guid colorId in command.Variants
            .Select(x => x.ColorId)
            .Distinct())
        {
            ProductColor productColor = product.AddColor(colorId);

            colors.Add(colorId, productColor);
        }

        foreach (CreateVariantRequest variant in command.Variants)
        {
            product.AddVariant(
                colors[variant.ColorId],
                variant.SizeId,
                variant.Price,
                variant.QuantityInStock,
                variant.SKU);
        }

        context.Products.Add(product);

        await context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }

    private async Task<Result> ValidateProductDetailsAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Result result = await validator.ValidateCategoryAsync(
            command.CategoryId,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        result = await validator.ValidateBrandAsync(
            command.BrandId,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        result = await validator.ValidateGenderIdsAsync(
            command.GenderIds,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        result = await validator.ValidateCategoryGenderCompatibilityAsync(
            command.CategoryId,
            command.GenderIds,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        result = await validator.ValidateColorIdsAsync(
            command.Variants.Select(x => x.ColorId).Distinct().ToList(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        result = await validator.ValidateSizeIdsAsync(
            command.Variants.Select(x => x.SizeId).Distinct().ToList(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        result = await validator.ValidateSkusAsync(
            command.Variants.Select(x => x.SKU).ToList(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        return Result.Success();
    }
}
