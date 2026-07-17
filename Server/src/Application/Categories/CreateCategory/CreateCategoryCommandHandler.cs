using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(
    IApplicationDbContext context,
    ProductValidator validator)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        bool nameExists = await context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Name == command.Name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(CategoryErrors.NameNotUnique);
        }

        var genderIds = command.GenderIds.Distinct().ToList();

        if (genderIds.Count > 0)
        {
            Result genderResult = await validator.ValidateGenderIdsAsync(genderIds, cancellationToken);

            if (genderResult.IsFailure)
            {
                return Result.Failure<Guid>(genderResult.Error);
            }
        }

        var category = new Category
        {
            Name = command.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim()
        };

        foreach (Guid genderId in genderIds)
        {
            category.AddGender(genderId);
        }

        context.Categories.Add(category);

        await context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
