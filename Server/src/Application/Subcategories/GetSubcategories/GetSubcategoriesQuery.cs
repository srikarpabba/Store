using Application.Abstractions.Messaging;

namespace Application.Subcategories.GetSubcategories;

public sealed record GetSubcategoriesQuery : IQuery<IReadOnlyList<SubcategoryResponse>>;
