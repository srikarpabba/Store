using Application.Abstractions.Messaging;

namespace Application.Brands.GetBrands;

public sealed record GetBrandsQuery : IQuery<IReadOnlyList<BrandResponse>>;
