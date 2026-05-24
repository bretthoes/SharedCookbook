using System.Threading.RateLimiting;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal static class DailyClientRateLimitPartitions
{
    internal static RateLimitPartition<string> Create(HttpContext httpContext, int dailyLimit) =>
        RateLimitPartition.GetFixedWindowLimiter(
            RateLimitPartitionKeys.GetClientKey(httpContext),
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = dailyLimit,
                Window = TimeSpan.FromDays(1),
                QueueLimit = 0
            });
}
