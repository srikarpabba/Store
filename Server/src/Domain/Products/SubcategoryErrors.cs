using SharedKernel;

namespace Domain.Products;

public static class SubcategoryErrors
{
    public static Error NotFound(Guid subcategoryId) => Error.NotFound(
        "Subcategories.NotFound",
        $"The subcategory with the Id = '{subcategoryId}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Subcategories.NameNotUnique",
        "A subcategory with this name already exists within its category.");

    public static readonly Error InUse = Error.Conflict(
        "Subcategories.InUse",
        "This subcategory is assigned to one or more products and cannot be deleted.");

    public static readonly Error NotInCategory = Error.Problem(
        "Subcategories.NotInCategory",
        "The subcategory does not belong to the selected category.");
}
