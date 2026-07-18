using API;
using API.Extensions;
using API.GraphQL;
using API.Handlers;
using Application;
using Hangfire;
using Hangfire.Dashboard;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddSwaggerDocumentation();

builder.Services
    .AddApplication()
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<ProductQueries>();

builder.Services.AddObservability(builder.Configuration, builder.Environment.ApplicationName);

builder.Services.AddRateLimitingInternal(builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();

    // Inspect queued/failed email jobs at /hangfire.
    // Default dashboard auth only allows loopback requests, which breaks
    // under Docker's port-forwarding — safe to allow all here since this
    // whole block is already Development-only.
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new AllowAllDashboardAuthorizationFilter()]
    });

    await app.ApplyMigrationsAsync();
}

app.UseCorrelationId();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseRateLimiter();

app.UseCors("Frontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapGraphQL();

app.MapEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();
