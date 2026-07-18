using Application.Abstractions.Messaging;
using Application.Common.Pagination;

namespace Application.Brands.GetBrands;

public sealed record GetBrandsQuery(int? PageIndex, int? PageSize) : IQuery<PagedResponse<BrandResponse>>;
