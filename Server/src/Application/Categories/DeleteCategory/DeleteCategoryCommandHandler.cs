using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await context.Categories
            .Include(c => c.CategoryGenders)
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(command.Id));
        }

        bool inUse = await context.Products
            .AsNoTracking()
            .AnyAsync(p => p.CategoryId == command.Id, cancellationToken);

        if (inUse)
        {
            return Result.Failure(CategoryErrors.InUse);
        }

        bool inUseBySubcategory = await context.Subcategories
            .AsNoTracking()
            .AnyAsync(s => s.CategoryId == command.Id, cancellationToken);

        if (inUseBySubcategory)
        {
            return Result.Failure(CategoryErrors.InUseBySubcategory);
        }

        var photoFileNames = category.CategoryGenders
            .Where(g => g.PhotoFileName is not null)
            .Select(g => g.PhotoFileName!)
            .ToList();

        context.Categories.Remove(category);

        await context.SaveChangesAsync(cancellationToken);

        // best effort — the category record is gone either way
        foreach (string photoFileName in photoFileNames)
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
