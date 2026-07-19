namespace Application.Categories;

public sealed record CategoryGenderResponse(
    Guid GenderId,
    string GenderName,
    string? Photo);

public sealed record CategorySizeResponse(
    Guid SizeId,
    string SizeName);

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<CategoryGenderResponse> Genders,
    IReadOnlyList<CategorySizeResponse> Sizes);
