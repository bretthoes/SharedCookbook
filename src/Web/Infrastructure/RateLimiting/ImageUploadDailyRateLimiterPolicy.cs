using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal sealed class ImageUploadDailyRateLimiterPolicy(
    ILogger<ImageUploadDailyRateLimiterPolicy> logger,
    IOptions<ImageUploadRateLimitOptions> options)
    : IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext) =>
        DailyClientRateLimitPartitions.Create(httpContext, options.Value.DailyLimit);

    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected =>
        async (context, cancellationToken) =>
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            logger.LogWarning(
                "User {UserId} reached the daily image upload rate limit ({DailyLimit}/day) on {Method} {Path}",
                userId,
                options.Value.DailyLimit,
                httpContext.Request.Method,
                httpContext.Request.Path);

            await RateLimiterRejectionHandler.RejectAsync(context, cancellationToken);
        };
}
