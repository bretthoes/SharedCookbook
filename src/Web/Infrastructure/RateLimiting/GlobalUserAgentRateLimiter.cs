using System.Threading.RateLimiting;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal static class GlobalUserAgentRateLimiter
{
    internal static PartitionedRateLimiter<HttpContext> Create() =>
        PartitionedRateLimiter.CreateChained(
            PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
            {
                var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                return RateLimitPartition.GetFixedWindowLimiter(userAgent, factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 4,
                        Window = TimeSpan.FromSeconds(2)
                    });
            }),
            PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
            {
                var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                return RateLimitPartition.GetFixedWindowLimiter(userAgent, factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 15,
                        Window = TimeSpan.FromSeconds(30)
                    });
            }));
}
