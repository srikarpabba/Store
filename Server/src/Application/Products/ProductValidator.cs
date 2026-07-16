using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products;

internal sealed class ProductValidator(IApplicationDbContext context)
{
    public async Task<Result> ValidateCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        bool exists = await context.Categories
            .AsNoTracking()
            .AnyAsync(x => x.Id == categoryId, cancellationToken);

        return exists
            ? Result.Success()
            : Result.Failure(
                Error.NotFound(
                    "Category.NotFound",
                    $"Category with Id '{categoryId}' was not found."));
    }

    public async Task<Result> ValidateBrandAsync(
        Guid brandId,
        CancellationToken cancellationToken)
    {
        bool exists = await context.Brands
            .AsNoTracking()
            .AnyAsync(x => x.Id == brandId, cancellationToken);

        return exists
            ? Result.Success()
            : Result.Failure(
                Error.NotFound(
                    "Brand.NotFound",
                    $"Brand with Id '{brandId}' was not found."));
    }

    public async Task<Result> ValidateGenderIdsAsync(
        IReadOnlyCollection<Guid> genderIds,
        CancellationToken cancellationToken)
    {
        int count = await context.Genders
            .AsNoTracking()
            .CountAsync(x => genderIds.Contains(x.Id), cancellationToken);

        return count == genderIds.Count
            ? Result.Success()
            : Result.Failure(
                Error.NotFound(
                    "Gender.NotFound",
                    "One or more genders were not found."));
    }

    public async Task<Result> ValidateColorIdsAsync(
        IReadOnlyCollection<Guid> colorIds,
        CancellationToken cancellationToken)
    {
        int count = await context.Colors
            .AsNoTracking()
            .CountAsync(x => colorIds.Contains(x.Id), cancellationToken);

        return count == colorIds.Count
            ? Result.Success()
            : Result.Failure(
                Error.NotFound(
                    "Color.NotFound",
                    "One or more colors were not found."));
    }

    public async Task<Result> ValidateSizeIdsAsync(
        IReadOnlyCollection<Guid> sizeIds,
        CancellationToken cancellationToken)
    {
        int count = await context.Sizes
            .AsNoTracking()
            .CountAsync(x => sizeIds.Contains(x.Id), cancellationToken);

        return count == sizeIds.Count
            ? Result.Success()
            : Result.Failure(
                Error.NotFound(
                    "Size.NotFound",
                    "One or more sizes were not found."));
    }

    public async Task<Result> ValidateSkusAsync(
        IReadOnlyCollection<string> skus,
        CancellationToken cancellationToken)
    {
        bool exists = await context.ProductVariants
            .AsNoTracking()
            .AnyAsync(x => skus.Contains(x.SKU), cancellationToken);

        return exists
            ? Result.Failure(
                Error.Conflict(
                    "ProductVariant.SKUExists",
                    "One or more SKUs already exist."))
            : Result.Success();
    }
}
