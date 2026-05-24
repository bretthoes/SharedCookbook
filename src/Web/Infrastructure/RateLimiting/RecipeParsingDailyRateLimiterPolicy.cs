using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal sealed class RecipeParsingDailyRateLimiterPolicy(
    ILogger<RecipeParsingDailyRateLimiterPolicy> logger,
    IOptions<RecipeParsingRateLimitOptions> options)
    : IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext) =>
        DailyClientRateLimitPartitions.Create(httpContext, options.Value.DailyLimit);

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        RateLimiterRejectionHandler.CreateDailyLimitHandler(
            logger,
            "recipe parsing",
            options.Value.DailyLimit);
}
