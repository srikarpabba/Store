namespace API.Extensions;

internal static class SwaggerApplicationExtensions
{
    internal static IApplicationBuilder UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();

        // Keeps the bearer token you paste into "Authorize" across page
        // reloads, instead of forgetting it every time
        app.UseSwaggerUI(o => o.EnablePersistAuthorization());

        return app;
    }
}
