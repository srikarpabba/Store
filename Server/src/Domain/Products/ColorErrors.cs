using SharedKernel;

namespace Domain.Products;

public static class ColorErrors
{
    public static Error NotFound(Guid colorId) => Error.NotFound(
        "Colors.NotFound",
        $"The color with the Id = '{colorId}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Colors.NameNotUnique",
        "A color with this name already exists.");

    public static readonly Error InUse = Error.Conflict(
        "Colors.InUse",
        "This color is assigned to one or more products and cannot be deleted.");
}
