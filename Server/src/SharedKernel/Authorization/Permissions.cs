namespace SharedKernel.Authorization;

public static class Permissions
{
    public const string UsersAccess = "users:access";
    public const string UsersUpdate = "users:update";

    public const string ProductsRead = "products:read";
    public const string ProductsCreate = "products:create";
    public const string ProductsUpdate = "products:update";
    public const string ProductsDelete = "products:delete";

    public const string CategoriesCreate = "categories:create";
    public const string CategoriesUpdate = "categories:update";
    public const string CategoriesDelete = "categories:delete";

    public const string SubcategoriesCreate = "subcategories:create";
    public const string SubcategoriesUpdate = "subcategories:update";
    public const string SubcategoriesDelete = "subcategories:delete";

    public const string BrandsCreate = "brands:create";
    public const string BrandsUpdate = "brands:update";
    public const string BrandsDelete = "brands:delete";

    public const string ColorsCreate = "colors:create";
    public const string ColorsUpdate = "colors:update";
    public const string ColorsDelete = "colors:delete";

    public const string SizesCreate = "sizes:create";
    public const string SizesUpdate = "sizes:update";
    public const string SizesDelete = "sizes:delete";

    public const string BannersRead = "banners:read";
    public const string BannersCreate = "banners:create";
    public const string BannersUpdate = "banners:update";
    public const string BannersDelete = "banners:delete";
}
