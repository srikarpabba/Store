namespace API.Extensions;

internal static class SwaggerApplicationExtensions
{
    internal static IApplicationBuilder UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
