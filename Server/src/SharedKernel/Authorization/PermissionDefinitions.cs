namespace SharedKernel.Authorization;

public static class PermissionDefinitions
{
    public static readonly IReadOnlyList<(string Name, string Description)> All =
    [
        (Permissions.UsersAccess, "Access users"),
        (Permissions.UsersUpdate, "Update users"),

        (Permissions.ProductsRead, "Read products"),
        (Permissions.ProductsCreate, "Create products"),
        (Permissions.ProductsUpdate, "Update products"),
        (Permissions.ProductsDelete, "Delete products"),

        (Permissions.CategoriesCreate, "Create categories"),
        (Permissions.CategoriesUpdate, "Update categories"),
        (Permissions.CategoriesDelete, "Delete categories"),

        (Permissions.BrandsCreate, "Create brands"),
        (Permissions.BrandsUpdate, "Update brands"),
        (Permissions.BrandsDelete, "Delete brands"),

        (Permissions.BannersRead, "Read banners"),
        (Permissions.BannersCreate, "Create banners"),
        (Permissions.BannersUpdate, "Update banners"),
        (Permissions.BannersDelete, "Delete banners")
    ];
}
