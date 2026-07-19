using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Subcategories.DeleteSubcategory;

internal sealed class DeleteSubcategoryCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteSubcategoryCommand>
{
    public async Task<Result> Handle(DeleteSubcategoryCommand command, CancellationToken cancellationToken)
    {
        Subcategory? subcategory = await context.Subcategories
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (subcategory is null)
        {
            return Result.Failure(SubcategoryErrors.NotFound(command.Id));
        }

        // Soft delete bypasses the FK RESTRICT, so enforce "in use" here.
        bool inUse = await context.Products
            .AsNoTracking()
            .AnyAsync(p => p.SubcategoryId == command.Id, cancellationToken);

        if (inUse)
        {
            return Result.Failure(SubcategoryErrors.InUse);
        }

        context.Subcategories.Remove(subcategory);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
