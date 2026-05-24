using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Web.Infrastructure;

public static class RateLimiterExtension
{
    internal static void AddRateLimiter(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<RecipeParsingRateLimitOptions>(
            builder.Configuration.GetSection(RecipeParsingRateLimitOptions.SectionName));

        builder.Services.AddSingleton<RecipeParsingDailyRateLimiterPolicy>();

        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = RateLimiterRejectionHandler.RejectAsync;
            options.AddPolicy<string, RecipeParsingDailyRateLimiterPolicy>(RateLimitPolicyNames.RecipeParsingDaily);
            options.GlobalLimiter = GlobalUserAgentRateLimiter.Create();
        });
    }
}
