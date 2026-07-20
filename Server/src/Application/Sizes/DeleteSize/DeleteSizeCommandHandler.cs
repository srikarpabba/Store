using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Sizes.DeleteSize;

internal sealed class DeleteSizeCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteSizeCommand>
{
    public async Task<Result> Handle(DeleteSizeCommand command, CancellationToken cancellationToken)
    {
        Size? size = await context.Sizes
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (size is null)
        {
            return Result.Failure(SizeErrors.NotFound(command.Id));
        }

        // Soft-deleted now, so the FK RESTRICT no longer guards against
        // orphaning an in-use size — enforce it in application code.
        bool inUse = await context.ProductVariants
            .AsNoTracking()
            .AnyAsync(v => v.SizeId == command.Id, cancellationToken);

        if (inUse)
        {
            return Result.Failure(SizeErrors.InUse);
        }

        bool inUseByCategory = await context.CategorySizes
            .AsNoTracking()
            .AnyAsync(cs => cs.SizeId == command.Id, cancellationToken);

        if (inUseByCategory)
        {
            return Result.Failure(SizeErrors.InUseByCategory);
        }

        context.Sizes.Remove(size);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
