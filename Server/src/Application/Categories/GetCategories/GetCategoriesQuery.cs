using Application.Abstractions.Messaging;
using Application.Common.Pagination;

namespace Application.Categories.GetCategories;

/// <summary>
/// <paramref name="Gender"/> is one of "Men" (tagged Male), "Women" (tagged
/// Female), "Unisex" (tagged Unisex), or null/omitted for all.
/// </summary>
public sealed record GetCategoriesQuery(int? PageIndex, int? PageSize, string? Gender) : IQuery<PagedResponse<CategoryResponse>>;
