using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Subcategories.CreateSubcategory;

internal sealed class CreateSubcategoryCommandHandler(IApplicationDbContext context, ProductValidator productValidator)
    : ICommandHandler<CreateSubcategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSubcategoryCommand command, CancellationToken cancellationToken)
    {
        Result categoryResult = await productValidator.ValidateCategoryAsync(command.CategoryId, cancellationToken);

        if (categoryResult.IsFailure)
        {
            return Result.Failure<Guid>(categoryResult.Error);
        }

        string name = command.Name.Trim();

        bool nameExists = await context.Subcategories
            .AsNoTracking()
            .AnyAsync(s => s.CategoryId == command.CategoryId && s.Name == name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(SubcategoryErrors.NameNotUnique);
        }

        var subcategory = new Subcategory
        {
            Name = name,
            CategoryId = command.CategoryId
        };

        context.Subcategories.Add(subcategory);

        await context.SaveChangesAsync(cancellationToken);

        return subcategory.Id;
    }
}
