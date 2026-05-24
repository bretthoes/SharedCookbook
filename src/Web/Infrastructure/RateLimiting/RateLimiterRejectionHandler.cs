using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal static class RateLimiterRejectionHandler
{
    internal static async ValueTask RejectAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    }

    internal static Func<OnRejectedContext, CancellationToken, ValueTask> CreateDailyLimitHandler(
        ILogger logger,
        string limitName,
        int dailyLimit) =>
        async (context, cancellationToken) =>
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            logger.LogWarning(
                "User {UserId} reached the daily {LimitName} rate limit ({DailyLimit}/day) on {Method} {Path}",
                userId,
                limitName,
                dailyLimit,
                httpContext.Request.Method,
                httpContext.Request.Path);

            await RejectAsync(context, cancellationToken);
        };
}
