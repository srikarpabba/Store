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
        List<CategoryGenderRow> Genders,
        List<CategorySizeResponse> Sizes);

    public async Task<Result<PagedResponse<CategoryResponse>>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        int pageIndex = query.PageIndex ?? 1;
        int pageSize = query.PageSize ?? DefaultPageSize;

        IQueryable<Category> categories = context.Categories.AsNoTracking();

        string? genderName = query.Gender switch
        {
            "Men" => "Male",
            "Women" => "Female",
            "Unisex" => "Unisex",
            _ => null
        };

        if (genderName is null)
        {
            categories = categories.OrderBy(c => c.Name);
        }
        else
        {
            // gender-filtered view mirrors that gender's storefront order,
            // so the admin sees (and can rearrange) what shoppers see
            categories = categories
                .Where(c => c.CategoryGenders.Any(cg => cg.Gender.Name == genderName))
                .OrderBy(c => c.CategoryGenders
                    .Where(cg => cg.Gender.Name == genderName)
                    .Select(cg => cg.SortOrder)
                    .FirstOrDefault())
                .ThenBy(c => c.Name);
        }

        int total = await categories.CountAsync(cancellationToken);

        List<CategoryRow> rows = await categories
            .ApplyPaging(pageIndex, pageSize)
            .Select(c => new CategoryRow(
                c.Id,
                c.Name,
                c.Description,
                c.CategoryGenders
                    .Select(cg => new CategoryGenderRow(cg.GenderId, cg.Gender.Name, cg.PhotoFileName))
                    .ToList(),
                c.CategorySizes
                    .Select(cs => new CategorySizeResponse(cs.SizeId, cs.Size.Name))
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
                    .ToList(),
                r.Sizes))
            .ToList();

        return new PagedResponse<CategoryResponse>(items, pageIndex, pageSize, total);
    }
}
