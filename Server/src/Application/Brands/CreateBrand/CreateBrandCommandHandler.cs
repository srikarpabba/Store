using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Brands.CreateBrand;

internal sealed class CreateBrandCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateBrandCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        bool nameExists = await context.Brands
            .AsNoTracking()
            .AnyAsync(b => b.Name == command.Name, cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(BrandErrors.NameNotUnique);
        }

        var brand = new Brand
        {
            Name = command.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim(),
            IsFeatured = command.IsFeatured
        };

        context.Brands.Add(brand);

        await context.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}
