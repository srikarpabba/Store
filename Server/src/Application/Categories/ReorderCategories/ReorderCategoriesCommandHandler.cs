using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.ReorderCategories;

internal sealed class ReorderCategoriesCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<ReorderCategoriesCommand>
{
    public async Task<Result> Handle(
        ReorderCategoriesCommand command,
        CancellationToken cancellationToken)
    {
        List<CategoryGender> tagged = await context.CategoryGenders
            .Where(cg => cg.GenderId == command.GenderId)
            .ToListAsync(cancellationToken);

        if (tagged.Count == 0)
        {
            return Result.Failure(Error.NotFound(
                "CategoryGenders.NotFound",
                $"No categories are tagged with gender '{command.GenderId}'."));
        }

        // The new order must cover exactly this gender's categories — no
        // missing ids, no strays, no duplicates.
        var orderedIds = command.CategoryIds.Distinct().ToList();

        if (orderedIds.Count != tagged.Count
            || tagged.Any(cg => !orderedIds.Contains(cg.CategoryId)))
        {
            return Result.Failure(Error.Problem(
                "CategoryGenders.OrderMismatch",
                "The category order must contain each of the gender's categories exactly once."));
        }

        foreach (CategoryGender categoryGender in tagged)
        {
            categoryGender.SetSortOrder(orderedIds.IndexOf(categoryGender.CategoryId));
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
