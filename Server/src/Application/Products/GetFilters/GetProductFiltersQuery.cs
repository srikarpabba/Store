using Application.Abstractions.Messaging;

namespace Application.Products.GetFilters;

public sealed record GetProductFiltersQuery : IQuery<ProductFiltersResponse>;
