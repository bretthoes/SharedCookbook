using SharedCookbook.Application.Subscriptions;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Web.Infrastructure;

public static class RateLimiterExtension
{
    internal static void AddRateLimiter(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<RecipeParsingRateLimitOptions>(
            builder.Configuration.GetSection(RecipeParsingRateLimitOptions.SectionName));

        builder.Services.Configure<ImageUploadRateLimitOptions>(
            builder.Configuration.GetSection(ImageUploadRateLimitOptions.SectionName));

        builder.Services.Configure<RevenueCatWebhookOptions>(
            builder.Configuration.GetSection(RevenueCatWebhookOptions.SectionName));

        builder.Services.AddSingleton<RecipeParsingDailyRateLimiterPolicy>();
        builder.Services.AddSingleton<ImageUploadDailyRateLimiterPolicy>();

        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = RateLimiterRejectionHandler.RejectAsync;
            options.AddPolicy<string, RecipeParsingDailyRateLimiterPolicy>(RateLimitPolicyNames.RecipeParsingDaily);
            options.AddPolicy<string, ImageUploadDailyRateLimiterPolicy>(RateLimitPolicyNames.ImageUploadDaily);
            options.GlobalLimiter = GlobalRequestRateLimiter.Create();
        });
    }
}
