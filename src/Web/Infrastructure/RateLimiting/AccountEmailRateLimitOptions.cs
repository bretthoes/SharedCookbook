namespace SharedCookbook.Web.Infrastructure.RateLimiting;

public sealed class AccountEmailRateLimitOptions
{
    public const string SectionName = "AccountEmailRateLimit";

    public int PermitLimit { get; init; } = 10;

    public int WindowMinutes { get; init; } = 15;
}
