using Application.Abstractions.Messaging;

namespace Application.Categories.GetCategories;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryResponse>>;
