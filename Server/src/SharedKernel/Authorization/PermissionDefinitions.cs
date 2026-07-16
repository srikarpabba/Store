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
        (Permissions.ProductsDelete, "Delete products")
    ];
}
