namespace SharedCookbook.Application.Common.Models;

public sealed record SubscriptionTierUpdateResult(
    Result Result,
    bool IsUserNotFound,
    bool WasUpdated);
