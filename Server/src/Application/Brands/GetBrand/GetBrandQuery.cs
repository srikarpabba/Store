using Application.Abstractions.Messaging;

namespace Application.Brands.GetBrand;

public sealed record GetBrandQuery(Guid Id) : IQuery<BrandResponse>;
