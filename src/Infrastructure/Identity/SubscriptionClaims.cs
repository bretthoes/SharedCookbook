namespace SharedCookbook.Infrastructure.Identity;

public static class SubscriptionClaims
{
    /// <summary>
    /// JWT claim that carries the user's subscription tier (e.g. "Free", "Pro").
    /// </summary>
    public const string Tier = "subscription_tier";
}
