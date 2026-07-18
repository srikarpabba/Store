using Hangfire.Dashboard;

namespace API.Handlers;

/// <summary>
/// Hangfire's default dashboard filter only allows requests whose remote IP
/// is loopback. Inside Docker, browser requests arrive via the container's
/// bridge network and never look local, so the default filter 401s every
/// request. The dashboard is already restricted to Development via
/// `IsDevelopment()` in Program.cs, so allowing all requests here is safe.
/// </summary>
internal sealed class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
