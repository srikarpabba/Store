using SharedKernel;

namespace Domain.Products;

public static class BrandErrors
{
    public static Error NotFound(Guid brandId) => Error.NotFound(
        "Brands.NotFound",
        $"The brand with the Id = '{brandId}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Brands.NameNotUnique",
        "A brand with this name already exists.");

    public static readonly Error InUse = Error.Conflict(
        "Brands.InUse",
        "This brand is assigned to one or more products and cannot be deleted.");

    public static readonly Error InUseByPromotion = Error.Conflict(
        "Brands.InUseByPromotion",
        "This brand has one or more promotions and cannot be deleted.");
}
