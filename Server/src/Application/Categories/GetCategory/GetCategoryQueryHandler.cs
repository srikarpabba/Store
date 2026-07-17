using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.GetCategory;

internal sealed class GetCategoryQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetCategoryQuery, CategoryResponse>
{
    private sealed record CategoryGenderRow(Guid GenderId, string GenderName, string? PhotoFileName);

    private sealed record CategoryRow(
        Guid Id,
        string Name,
        string? Description,
        List<CategoryGenderRow> Genders);

    public async Task<Result<CategoryResponse>> Handle(
        GetCategoryQuery query,
        CancellationToken cancellationToken)
    {
        CategoryRow? category = await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(c => new CategoryRow(
                c.Id,
                c.Name,
                c.Description,
                c.CategoryGenders
                    .Select(cg => new CategoryGenderRow(cg.GenderId, cg.Gender.Name, cg.PhotoFileName))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound(query.Id));
        }

        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.Genders
                .Select(g => new CategoryGenderResponse(
                    g.GenderId,
                    g.GenderName,
                    g.PhotoFileName is null ? null : fileStorage.GetUrl(g.PhotoFileName).AbsoluteUri))
                .ToList());
    }
}
