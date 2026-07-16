using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace API.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string Header = "X-Correlation-Id";

    public async Task Invoke(HttpContext context)
    {
        string correlationId =
            context.Request.Headers.TryGetValue(Header, out StringValues value)
                ? value.ToString()
                : Guid.NewGuid().ToString();

        context.Items[Header] = correlationId;
        context.Response.Headers[Header] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
