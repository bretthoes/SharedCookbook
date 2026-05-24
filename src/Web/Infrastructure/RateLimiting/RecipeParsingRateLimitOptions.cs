namespace SharedCookbook.Web.Infrastructure.RateLimiting;

public sealed class RecipeParsingRateLimitOptions
{
    public const string SectionName = "RecipeParsingRateLimit";

    /// <summary>
    /// Daily cap for Pro subscribers. High abuse-prevention ceiling.
    /// </summary>
    public int DailyLimit { get; set; } = 100;

    /// <summary>
    /// Daily cap for Free-tier users (~5 imports/week × 3 days buffer).
    /// </summary>
    public int FreeDailyLimit { get; set; } = 15;
}
