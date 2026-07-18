namespace API.Extensions;

internal static class SecurityHeadersExtensions
{
    internal static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            IHeaderDictionary headers = context.Response.Headers;

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";

            // Swagger UI and the Hangfire dashboard (both Development-only) render HTML
            // with inline scripts/styles that a locked-down CSP would break; everywhere
            // else this is a pure JSON API, so lock it down there.
            if (!app.Environment.IsDevelopment())
            {
                headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
            }

            await next();
        });

        return app;
    }
}
