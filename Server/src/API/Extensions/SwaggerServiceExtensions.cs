using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace API.Extensions;

internal static class SwaggerServiceExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(static o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Store API",
                Version = "v1",
                Description = "REST API for the Store storefront and admin catalog management."
            });

            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            // Request DTOs use nullable reference types (e.g. `string? Description`)
            // to mean "optional" — without this, Swashbuckle can't see that
            // signal and schemas mark optional fields as required.
            o.SupportNonNullableReferenceTypes();

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            o.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document),
                    []
                }
            });
        });

        return services;
    }
}
