using SharedKernel;

namespace Domain.Products;

public static class CategoryErrors
{
    public static Error NotFound(Guid categoryId) => Error.NotFound(
        "Categories.NotFound",
        $"The category with the Id = '{categoryId}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Categories.NameNotUnique",
        "A category with this name already exists.");

    public static readonly Error InUse = Error.Conflict(
        "Categories.InUse",
        "This category is assigned to one or more products and cannot be deleted.");

    public static readonly Error InUseBySubcategory = Error.Conflict(
        "Categories.InUseBySubcategory",
        "This category has one or more subcategories and cannot be deleted.");

    public static Error GenderNotAssociated(Guid categoryId, Guid genderId) => Error.NotFound(
        "Categories.GenderNotAssociated",
        $"Gender '{genderId}' is not associated with category '{categoryId}'.");
}
