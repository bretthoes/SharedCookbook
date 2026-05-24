using System.Security.Claims;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal static class RateLimitPartitionKeys
{
    /// <summary>
    /// Authenticated user id, else client IP, else a shared anonymous bucket.
    /// </summary>
    internal static string GetClientKey(HttpContext httpContext) =>
        httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContext.Connection.RemoteIpAddress?.ToString()
        ?? "anonymous";
}
