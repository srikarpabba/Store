using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Common.Pagination;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetCategoriesQuery, PagedResponse<CategoryResponse>>
{
    private const int DefaultPageSize = 25;

    private sealed record CategoryGenderRow(Guid GenderId, string GenderName, string? PhotoFileName);

    private sealed record CategoryRow(
        Guid Id,
        string Name,
        string? Description,
        List<CategoryGenderRow> Genders);

    public async Task<Result<PagedResponse<CategoryResponse>>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        int pageIndex = query.PageIndex ?? 1;
        int pageSize = query.PageSize ?? DefaultPageSize;

        IQueryable<Category> categories = context.Categories.AsNoTracking();

        categories = query.Gender switch
        {
            "Men" => categories.Where(c => c.CategoryGenders.Any(cg => cg.Gender.Name == "Male")),
            "Women" => categories.Where(c => c.CategoryGenders.Any(cg => cg.Gender.Name == "Female")),
            "Unisex" => categories.Where(c => c.CategoryGenders.Any(cg => cg.Gender.Name == "Unisex")),
            _ => categories
        };

        categories = categories.OrderBy(c => c.Name);

        int total = await categories.CountAsync(cancellationToken);

        List<CategoryRow> rows = await categories
            .ApplyPaging(pageIndex, pageSize)
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

        return new PagedResponse<CategoryResponse>(items, pageIndex, pageSize, total);
    }
}
