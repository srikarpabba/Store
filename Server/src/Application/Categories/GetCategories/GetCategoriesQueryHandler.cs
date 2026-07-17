using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    private sealed record CategoryGenderRow(Guid GenderId, string GenderName, string? PhotoFileName);

    private sealed record CategoryRow(
        Guid Id,
        string Name,
        string? Description,
        List<CategoryGenderRow> Genders);

    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        List<CategoryRow> rows = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryRow(
                c.Id,
                c.Name,
                c.Description,
                c.CategoryGenders
                    .Select(cg => new CategoryGenderRow(cg.GenderId, cg.Gender.Name, cg.PhotoFileName))
                    .ToList()))
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new CategoryResponse(
                r.Id,
                r.Name,
                r.Description,
                r.Genders
                    .Select(g => new CategoryGenderResponse(
                        g.GenderId,
                        g.GenderName,
                        g.PhotoFileName is null ? null : fileStorage.GetUrl(g.PhotoFileName).AbsoluteUri))
                    .ToList()))
            .ToList();

        return items;
    }
}
