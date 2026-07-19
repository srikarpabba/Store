using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Subcategories.UpdateSubcategory;

internal sealed class UpdateSubcategoryCommandHandler(IApplicationDbContext context, ProductValidator productValidator)
    : ICommandHandler<UpdateSubcategoryCommand>
{
    public async Task<Result> Handle(UpdateSubcategoryCommand command, CancellationToken cancellationToken)
    {
        Subcategory? subcategory = await context.Subcategories
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (subcategory is null)
        {
            return Result.Failure(SubcategoryErrors.NotFound(command.Id));
        }

        Result categoryResult = await productValidator.ValidateCategoryAsync(command.CategoryId, cancellationToken);

        if (categoryResult.IsFailure)
        {
            return categoryResult;
        }

        string name = command.Name.Trim();

        bool nameExists = await context.Subcategories
            .AsNoTracking()
            .AnyAsync(s => s.Id != command.Id && s.CategoryId == command.CategoryId && s.Name == name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure(SubcategoryErrors.NameNotUnique);
        }

        subcategory.Update(name, command.CategoryId);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
