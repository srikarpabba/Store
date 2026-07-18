using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Sizes.UpdateSize;

internal sealed class UpdateSizeCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateSizeCommand>
{
    public async Task<Result> Handle(UpdateSizeCommand command, CancellationToken cancellationToken)
    {
        Size? size = await context.Sizes
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (size is null)
        {
            return Result.Failure(SizeErrors.NotFound(command.Id));
        }

        string name = command.Name.Trim();

        bool nameExists = await context.Sizes
            .AsNoTracking()
            .AnyAsync(s => s.Id != command.Id && s.Name == name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure(SizeErrors.NameNotUnique);
        }

        size.Update(name);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
