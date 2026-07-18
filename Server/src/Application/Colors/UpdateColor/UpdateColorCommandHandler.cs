using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Colors.UpdateColor;

internal sealed class UpdateColorCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateColorCommand>
{
    public async Task<Result> Handle(UpdateColorCommand command, CancellationToken cancellationToken)
    {
        Color? color = await context.Colors
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (color is null)
        {
            return Result.Failure(ColorErrors.NotFound(command.Id));
        }

        string name = command.Name.Trim();

        bool nameExists = await context.Colors
            .AsNoTracking()
            .AnyAsync(c => c.Id != command.Id && c.Name == name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure(ColorErrors.NameNotUnique);
        }

        color.Update(name, command.HexCode.Trim().ToUpperInvariant());

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
