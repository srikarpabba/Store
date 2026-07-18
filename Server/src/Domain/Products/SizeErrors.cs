using SharedKernel;

namespace Domain.Products;

public static class SizeErrors
{
    public static Error NotFound(Guid sizeId) => Error.NotFound(
        "Sizes.NotFound",
        $"The size with the Id = '{sizeId}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Sizes.NameNotUnique",
        "A size with this name already exists.");

    public static readonly Error InUse = Error.Conflict(
        "Sizes.InUse",
        "This size is assigned to one or more product variants and cannot be deleted.");
}
