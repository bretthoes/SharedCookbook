namespace SharedCookbook.Application.Notifications.FanOut;

/// <summary>
/// Decides which cookbook members receive a notification.
/// </summary>
internal static class NotificationRecipientRules
{
    public static HashSet<string> UserIdsToSkip(CookbookNotificationActionType actionType, string? subjectUserId) =>
        actionType switch
        {
            // No longer a member, but fan-out runs before the save finishes, so this membership
            // can still appear in the member query and is skipped here.
            CookbookNotificationActionType.MemberRemoved => Skip(subjectUserId),

            // No longer a member of this cookbook, so they do not receive its notifications.
            // Fan-out runs before the save finishes, so their membership can still appear in the query.
            CookbookNotificationActionType.MemberLeft => Skip(subjectUserId),

            _ => Nobody(),
        };

    private static HashSet<string> Skip(string? userId)
    {
        var skip = Nobody();
        if (userId is not null)
            skip.Add(userId);
        return skip;
    }

    private static HashSet<string> Nobody() => new(StringComparer.Ordinal);
}
