using API.Endpoints;
using API.Endpoints.Auth;
using API.Endpoints.Banners;
using API.Endpoints.Brands;
using API.Endpoints.Categories;
using API.Endpoints.Products;
using API.Endpoints.Storefronts;
using API.Endpoints.Users;
using API.RateLimiting;

namespace API.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/api");

        endpoints.MapStorefrontEndpoints();
        endpoints.MapAuthenticationEndpoints();
        endpoints.MapProductEndpoints();
        endpoints.MapCategoryEndpoints();
        endpoints.MapBrandEndpoints();
        endpoints.MapBannerEndpoints();
        endpoints.MapCartEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapWishlistEndpoints();
    }

    private static void MapStorefrontEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/storefronts")
            .WithTags(Tags.Storefronts)
            .RequireRateLimiting(RateLimitingPolicies.Authentication);

        endpoints.MapPublicGroup()
           .MapEndpoint<GetStorefrontSections>();
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

    private static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/categories")
            .WithTags(Tags.Categories);

        endpoints.MapPublicGroup()
           .MapEndpoint<GetCategories>()
           .MapEndpoint<GetCategory>();

        endpoints.MapAuthorizedGroup()
           .MapEndpoint<CreateCategory>()
           .MapEndpoint<UpdateCategory>()
           .MapEndpoint<DeleteCategory>()
           .MapEndpoint<UploadCategoryGenderPhoto>()
           .MapEndpoint<DeleteCategoryGenderPhoto>();
    }

    private static void MapBrandEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/brands")
            .WithTags(Tags.Brands);

        endpoints.MapPublicGroup()
           .MapEndpoint<GetBrands>()
           .MapEndpoint<GetBrand>();

        endpoints.MapAuthorizedGroup()
           .MapEndpoint<CreateBrand>()
           .MapEndpoint<UpdateBrand>()
           .MapEndpoint<DeleteBrand>()
           .MapEndpoint<UploadBrandLogo>()
           .MapEndpoint<DeleteBrandLogo>();
    }

    private static void MapBannerEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/banners")
            .WithTags(Tags.Banners);

        // Admin-only end to end — the storefront-facing banner list comes
        // from /storefronts/{storefront} instead, so there's no public group here
        endpoints.MapAuthorizedGroup()
           .MapEndpoint<GetBanners>()
           .MapEndpoint<GetBanner>()
           .MapEndpoint<CreateBanner>()
           .MapEndpoint<UpdateBanner>()
           .MapEndpoint<DeleteBanner>()
           .MapEndpoint<UploadBannerImage>();
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
