using API.Endpoints;
using API.Endpoints.Auth;
using API.Endpoints.Home;
using API.Endpoints.Products;
using API.Endpoints.Users;
using API.RateLimiting;

namespace API.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/api");

        endpoints.MapHomeEndpoints();
        endpoints.MapAuthenticationEndpoints();
        endpoints.MapProductEndpoints();
        endpoints.MapCartEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapWishlistEndpoints();
    }

    private static void MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/home")
            .WithTags(Tags.Home)
            .RequireRateLimiting(RateLimitingPolicies.Authentication);

        endpoints.MapPublicGroup()
           .MapEndpoint<GetHome>();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/auth")
            .WithTags(Tags.Auth)
            .RequireRateLimiting(RateLimitingPolicies.Authentication);

        endpoints.MapPublicGroup()
           .MapEndpoint<Register>()
           .MapEndpoint<Login>()
           .MapEndpoint<GoogleLogin>()
           .MapEndpoint<RefreshToken>()
           .MapEndpoint<ForgotPassword>()
           .MapEndpoint<ResetPassword>()
           .MapEndpoint<ConfirmEmail>();

        endpoints.MapAuthorizedGroup()
           .MapEndpoint<ChangePassword>()
           .MapEndpoint<SetPassword>();
    }

    private static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/products")
            .WithTags(Tags.Products);

        endpoints.MapPublicGroup()
           .MapEndpoint<GetProducts>()
           .MapEndpoint<GetProduct>()
           .MapEndpoint<GetProductFilters>();

        endpoints.MapAuthorizedGroup()
           .MapEndpoint<CreateProduct>()
           .MapEndpoint<UpdateProduct>()
           .MapEndpoint<DeleteProduct>()
           .MapEndpoint<UploadProductImages>()
           .MapEndpoint<DeleteProductImage>()
           .MapEndpoint<SetMainProductImage>();
    }

    private static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        //TODO
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/users")
            .WithTags(Tags.Users);

        endpoints.MapAuthorizedGroup()
           .MapEndpoint<GetMyProfile>()
           .MapEndpoint<UpdateMyProfile>()
           .MapEndpoint<ResendEmailConfirmation>()
           .MapEndpoint<GetMyAddresses>()
           .MapEndpoint<AddAddress>()
           .MapEndpoint<UpdateAddress>()
           .MapEndpoint<DeleteAddress>()
           .MapEndpoint<SetDefaultAddress>();
    }

    private static void MapWishlistEndpoints(this IEndpointRouteBuilder app)
    {
        //TODO
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
