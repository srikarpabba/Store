using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Sizes.CreateSize;

internal sealed class CreateSizeCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateSizeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSizeCommand command, CancellationToken cancellationToken)
    {
        string name = command.Name.Trim();

        bool nameExists = await context.Sizes
            .AsNoTracking()
            .AnyAsync(s => s.Name == name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(SizeErrors.NameNotUnique);
        }

        var size = new Size
        {
            Name = name
        };

        context.Sizes.Add(size);

        await context.SaveChangesAsync(cancellationToken);

        return size.Id;
    }
}
