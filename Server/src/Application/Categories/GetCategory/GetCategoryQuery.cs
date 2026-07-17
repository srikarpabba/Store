using Application.Abstractions.Messaging;

namespace Application.Categories.GetCategory;

public sealed record GetCategoryQuery(Guid Id) : IQuery<CategoryResponse>;
