using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Products;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(
    IApplicationDbContext context,
    ProductValidator validator,
    IFileStorage fileStorage)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await context.Categories
            .Include(c => c.CategoryGenders)
            .Include(c => c.CategorySizes)
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(command.Id));
        }

        bool nameExists = await context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id != command.Id && c.Name == command.Name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure(CategoryErrors.NameNotUnique);
        }

        var genderIds = command.GenderIds.Distinct().ToList();

        Result genderResult = await validator.ValidateGenderIdsAsync(genderIds, cancellationToken);

        if (genderResult.IsFailure)
        {
            return genderResult;
        }

        var sizeIds = command.SizeIds.Distinct().ToList();

        if (sizeIds.Count > 0)
        {
            Result sizeResult = await validator.ValidateSizeIdsAsync(sizeIds, cancellationToken);

            if (sizeResult.IsFailure)
            {
                return sizeResult;
            }
        }

        category.Update(
            command.Name.Trim(),
            string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim());

        var removedGenders = category.CategoryGenders
            .Where(g => !genderIds.Contains(g.GenderId))
            .ToList();

        foreach (CategoryGender gender in removedGenders)
        {
            category.RemoveGender(gender);
        }

        foreach (Guid genderId in genderIds
            .Where(id => !category.CategoryGenders.Any(g => g.GenderId == id)))
        {
            category.AddGender(genderId);
        }

        foreach (CategorySize size in category.CategorySizes
            .Where(s => !sizeIds.Contains(s.SizeId))
            .ToList())
        {
            category.RemoveSize(size);
        }

        foreach (Guid sizeId in sizeIds
            .Where(id => !category.CategorySizes.Any(s => s.SizeId == id)))
        {
            category.AddSize(sizeId);
        }

        await context.SaveChangesAsync(cancellationToken);

        // best effort — the category-gender rows are already gone either way
        foreach (string photoFileName in removedGenders
            .Where(g => g.PhotoFileName is not null)
            .Select(g => g.PhotoFileName!))
        {
            try
            {
                await fileStorage.DeleteAsync(photoFileName, cancellationToken);
            }
            catch (Exception)
            {
                // swallow: blob cleanup can be retried out of band
            }
        }

        return Result.Success();
    }
}
