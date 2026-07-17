using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.UpdateBrand;

internal sealed class UpdateBrandCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateBrandCommand>
{
    public async Task<Result> Handle(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        Brand? brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (brand is null)
        {
            return Result.Failure(BrandErrors.NotFound(command.Id));
        }

        bool nameExists = await context.Brands
            .AsNoTracking()
            .AnyAsync(b => b.Id != command.Id && b.Name == command.Name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure(BrandErrors.NameNotUnique);
        }

        brand.Update(
            command.Name.Trim(),
            string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim(),
            command.IsFeatured);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
