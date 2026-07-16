namespace API.Extensions;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder HasPermission(
        this RouteHandlerBuilder builder,
        string permission)
    {
        return builder.RequireAuthorization(permission);
    }
}
