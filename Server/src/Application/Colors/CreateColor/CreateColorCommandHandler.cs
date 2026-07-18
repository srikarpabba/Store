using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Colors.CreateColor;

internal sealed class CreateColorCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateColorCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateColorCommand command, CancellationToken cancellationToken)
    {
        string name = command.Name.Trim();

        bool nameExists = await context.Colors
            .AsNoTracking()
            .AnyAsync(c => c.Name == name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(ColorErrors.NameNotUnique);
        }

        var color = new Color
        {
            Name = name,
            HexCode = command.HexCode.Trim().ToUpperInvariant()
        };

        context.Colors.Add(color);

        await context.SaveChangesAsync(cancellationToken);

        return color.Id;
    }
}
