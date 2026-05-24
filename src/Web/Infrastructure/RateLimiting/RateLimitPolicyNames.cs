namespace SharedCookbook.Web.Infrastructure.RateLimiting;

public static class RateLimitPolicyNames
{
    /// <summary>
    /// Shared daily quota for recipe parsing endpoints that call external APIs
    /// (URL, image OCR, voice/AI).
    /// </summary>
    public const string RecipeParsingDaily = nameof(RecipeParsingDaily);

    /// <summary>
    /// Daily quota for direct image uploads to S3.
    /// </summary>
    public const string ImageUploadDaily = nameof(ImageUploadDaily);
}
