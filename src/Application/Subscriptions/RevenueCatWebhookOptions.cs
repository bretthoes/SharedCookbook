namespace SharedCookbook.Application.Subscriptions;

public sealed class RevenueCatWebhookOptions
{
    public const string SectionName = "RevenueCat";

    /// <summary>
    /// Shared secret configured in the RevenueCat dashboard (Webhooks → Authorization header value).
    /// Leave empty to skip validation in development.
    /// </summary>
    public string SharedSecret { get; set; } = string.Empty;
}
