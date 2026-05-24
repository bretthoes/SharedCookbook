using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal sealed class RecipeParsingDailyRateLimiterPolicy(
    ILogger<RecipeParsingDailyRateLimiterPolicy> logger,
    IOptions<RecipeParsingRateLimitOptions> options)
    : IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var partitionKey = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
            new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = options.Value.DailyLimit,
                Window = TimeSpan.FromDays(1),
                QueueLimit = 0
            });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        async (context, cancellationToken) =>
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            logger.LogWarning(
                "User {UserId} reached the daily recipe parsing rate limit ({DailyLimit}/day) on {Method} {Path}",
                userId,
                options.Value.DailyLimit,
                httpContext.Request.Method,
                httpContext.Request.Path);

            await RateLimiterRejectionHandler.RejectAsync(context, cancellationToken);
        };
}
