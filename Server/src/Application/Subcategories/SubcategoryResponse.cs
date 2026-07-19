namespace Application.Subcategories;

public sealed record SubcategoryResponse(
    Guid Id,
    string Name,
    Guid CategoryId,
    string CategoryName);
