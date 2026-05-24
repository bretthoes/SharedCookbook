using System.Threading.RateLimiting;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal static class GlobalRequestRateLimiter
{
    internal static PartitionedRateLimiter<HttpContext> Create() =>
        PartitionedRateLimiter.CreateChained(
            PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
            {
                var partitionKey = RateLimitPartitionKeys.GetClientKey(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 30,
                        Window = TimeSpan.FromSeconds(10)
                    });
            }),
            PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
            {
                var partitionKey = RateLimitPartitionKeys.GetClientKey(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1)
                    });
            }));
}
