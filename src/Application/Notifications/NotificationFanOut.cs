namespace SharedCookbook.Application.Notifications;

public sealed class NotificationFanOut(IApplicationDbContext context) : INotificationFanOut
{
    public async Task FanOutAsync(
        int cookbookId,
        string actorUserId,
        CookbookNotificationActionType actionType,
        int? recipeId = null,
        string? subjectUserId = null,
        CancellationToken ct = default)
    {
        var exclude = new HashSet<string>(StringComparer.Ordinal) { actorUserId };
        if (subjectUserId is not null)
            exclude.Add(subjectUserId);

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
                    RecipeId = recipeId,
                },
                ct);
        }
    }
}
