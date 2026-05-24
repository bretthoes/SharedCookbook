namespace SharedCookbook.Web.Infrastructure.RateLimiting;

public sealed class ImageUploadRateLimitOptions
{
    public const string SectionName = "ImageUploadRateLimit";

    /// <summary>
    /// Abuse-prevention cap for S3 storage costs.
    /// </summary>
    public int DailyLimit { get; set; } = 100;
}
