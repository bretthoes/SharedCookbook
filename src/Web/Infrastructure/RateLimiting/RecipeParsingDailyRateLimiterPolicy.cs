using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

internal sealed class RecipeParsingDailyRateLimiterPolicy(
    ILogger<RecipeParsingDailyRateLimiterPolicy> logger,
    IOptions<RecipeParsingRateLimitOptions> options)
    : IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var tier = httpContext.User.FindFirstValue(SubscriptionClaims.Tier)
                   ?? nameof(SubscriptionTier.Free);
        var isPro = tier == nameof(SubscriptionTier.Pro);
        var limit = isPro ? options.Value.DailyLimit : options.Value.FreeDailyLimit;

        var baseKey = RateLimitPartitionKeys.GetClientKey(httpContext);
        var partitionKey = $"{baseKey}:{tier}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = limit,
                Window = TimeSpan.FromDays(1),
                QueueLimit = 0,
            });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        RateLimiterRejectionHandler.CreateDailyLimitHandler(
            logger,
            "recipe parsing",
            options.Value.DailyLimit);
}
