using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Subcategories.GetSubcategory;

internal sealed class GetSubcategoryQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSubcategoryQuery, SubcategoryResponse>
{
    public async Task<Result<SubcategoryResponse>> Handle(GetSubcategoryQuery query, CancellationToken cancellationToken)
    {
        SubcategoryResponse? subcategory = await context.Subcategories
            .AsNoTracking()
            .Where(s => s.Id == query.Id)
            .Select(s => new SubcategoryResponse(s.Id, s.Name, s.CategoryId, s.Category.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (subcategory is null)
        {
            return Result.Failure<SubcategoryResponse>(SubcategoryErrors.NotFound(query.Id));
        }

        return subcategory;
    }
}
