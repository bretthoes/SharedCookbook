using System.Globalization;
using System.Threading.RateLimiting;

namespace SharedCookbook.Web.Infrastructure;

public static class RateLimiterExtension
{
    internal static void AddRateLimiter(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }
                
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
            };
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
                {
                    var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter
                    (userAgent, factory: _ =>
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
            
                    return RateLimitPartition.GetFixedWindowLimiter
                    (userAgent, factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 15,
                            Window = TimeSpan.FromSeconds(30)
                        });
                }));
        });
    }
}
