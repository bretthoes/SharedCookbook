namespace SharedCookbook.Web.Infrastructure.RateLimiting;

public sealed class RecipeParsingRateLimitOptions
{
    public const string SectionName = "RecipeParsingRateLimit";

    /// <summary>
    /// Abuse-prevention cap for paid external API usage — not meant to constrain normal use.
    /// </summary>
    public int DailyLimit { get; set; } = 100;
}
