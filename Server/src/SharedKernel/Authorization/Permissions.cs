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

    public const string BrandsCreate = "brands:create";
    public const string BrandsUpdate = "brands:update";
    public const string BrandsDelete = "brands:delete";
}
