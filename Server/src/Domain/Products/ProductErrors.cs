using SharedKernel;

namespace Domain.Products;

public static class ProductErrors
{
    public static Error NotFound(Guid productId) => Error.NotFound(
        "Products.NotFound",
        $"The product with the Id = '{productId}' was not found");

    public static Error VariantNotFound(Guid variantId) => Error.NotFound(
        "Products.VariantNotFound",
        $"The product variant with the Id = '{variantId}' was not found");

    public static Error PhotoNotFound(Guid photoId) => Error.NotFound(
        "Products.PhotoNotFound",
        $"The product photo with the Id = '{photoId}' was not found");

    public static readonly Error CategoryGenderMismatch = Error.Validation(
        "Products.CategoryGenderMismatch",
        "One or more selected genders are not valid for this category.");

    public static readonly Error InUseByPromotion = Error.Conflict(
        "Products.InUseByPromotion",
        "This product has one or more promotions and cannot be deleted.");
}
