using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Colors.DeleteColor;

internal sealed class DeleteColorCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteColorCommand>
{
    public async Task<Result> Handle(DeleteColorCommand command, CancellationToken cancellationToken)
    {
        Color? color = await context.Colors
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (color is null)
        {
            return Result.Failure(ColorErrors.NotFound(command.Id));
        }

        // Color/Size are now soft-deleted, so the FK RESTRICT no longer
        // protects against orphaning an in-use color — enforce it here.
        bool inUse = await context.Products
            .AsNoTracking()
            .AnyAsync(p => p.ProductColors.Any(pc => pc.ColorId == command.Id), cancellationToken);

        if (inUse)
        {
            return Result.Failure(ColorErrors.InUse);
        }

        context.Colors.Remove(color);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
