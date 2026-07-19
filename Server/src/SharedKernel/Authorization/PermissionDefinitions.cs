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

        (Permissions.SubcategoriesCreate, "Create subcategories"),
        (Permissions.SubcategoriesUpdate, "Update subcategories"),
        (Permissions.SubcategoriesDelete, "Delete subcategories"),

        (Permissions.BrandsCreate, "Create brands"),
        (Permissions.BrandsUpdate, "Update brands"),
        (Permissions.BrandsDelete, "Delete brands"),

        (Permissions.ColorsCreate, "Create colors"),
        (Permissions.ColorsUpdate, "Update colors"),
        (Permissions.ColorsDelete, "Delete colors"),

        (Permissions.SizesCreate, "Create sizes"),
        (Permissions.SizesUpdate, "Update sizes"),
        (Permissions.SizesDelete, "Delete sizes"),

        (Permissions.BannersRead, "Read banners"),
        (Permissions.BannersCreate, "Create banners"),
        (Permissions.BannersUpdate, "Update banners"),
        (Permissions.BannersDelete, "Delete banners")
    ];
}
