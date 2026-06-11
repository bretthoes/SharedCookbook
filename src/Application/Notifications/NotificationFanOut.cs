namespace SharedCookbook.Application.Notifications;

// TODO revisit where this file should live; it's possible it can be here, honoring vertical slice this is the home for other notification functionality. But it isn't called by any of the notification handlers (queries or commands) defined here. It is called by multiple event handlers from different domains.
public sealed class NotificationFanOut(IApplicationDbContext context, IIdentityService identityService) : INotificationFanOut
{
    public async Task FanOutAsync(
        Guid cookbookId,
        string actorUserId,
        CookbookNotificationActionType actionType,
        Guid? recipeId = null,
        string? subjectUserId = null,
        CancellationToken ct = default)
    {
        var exclude = new HashSet<string>(StringComparer.Ordinal) { actorUserId };
        if (subjectUserId is not null)
            exclude.Add(subjectUserId);

        var actorDisplayName = await identityService.GetDisplayNameAsync(actorUserId, ct);
        string? subjectDisplayName = subjectUserId is not null
            ? await identityService.GetDisplayNameAsync(subjectUserId, ct)
            : null;

        // TODO this is a db query; move to a dedicated db query file
        var recipientIds = await context.CookbookMemberships
            .Where(m => m.CookbookId == cookbookId && m.CreatedBy != null && !exclude.Contains(m.CreatedBy))
            .Select(m => m.CreatedBy!)
            .Distinct()
            .ToListAsync(ct);

        foreach (var recipientId in recipientIds)
        {
            await context.CookbookNotifications.AddAsync(
                new CookbookNotification
                {
                    RecipientUserId = recipientId,
                    CookbookId = cookbookId,
                    ActionType = actionType,
                    ActorUserId = actorUserId,
                    SubjectUserId = subjectUserId,
                    ActorDisplayName = actorDisplayName,
                    SubjectDisplayName = subjectDisplayName,
                    RecipeId = recipeId,
                },
                ct);
        }
    }
}
