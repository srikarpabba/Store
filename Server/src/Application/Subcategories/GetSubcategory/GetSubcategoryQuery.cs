using Application.Abstractions.Messaging;

namespace Application.Subcategories.GetSubcategory;

public sealed record GetSubcategoryQuery(Guid Id) : IQuery<SubcategoryResponse>;
