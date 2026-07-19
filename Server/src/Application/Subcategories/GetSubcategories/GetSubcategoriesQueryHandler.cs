using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Subcategories.GetSubcategories;

internal sealed class GetSubcategoriesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSubcategoriesQuery, IReadOnlyList<SubcategoryResponse>>
{
    public async Task<Result<IReadOnlyList<SubcategoryResponse>>> Handle(
        GetSubcategoriesQuery query,
        CancellationToken cancellationToken)
    {
        List<SubcategoryResponse> subcategories = await context.Subcategories
            .AsNoTracking()
            .OrderBy(s => s.Category.Name)
            .ThenBy(s => s.Name)
            .Select(s => new SubcategoryResponse(s.Id, s.Name, s.CategoryId, s.Category.Name))
            .ToListAsync(cancellationToken);

        return subcategories;
    }
}
