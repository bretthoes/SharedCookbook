namespace SharedCookbook.Application.Notifications.FanOut;

public sealed class NotificationFanOut(IApplicationDbContext context, IIdentityService identityService) : INotificationFanOut
{
    /// <inheritdoc />
    public async Task FanOutAsync(
        Guid cookbookId,
        string actorUserId,
        CookbookNotificationActionType actionType,
        Guid? recipeId = null,
        string? subjectUserId = null,
        CancellationToken ct = default)
    {
        var excluded = NotificationRecipientRules.UserIdsToSkip(actionType, subjectUserId);

        var actorDisplayName = await identityService.GetDisplayNameAsync(actorUserId, ct);
        string? subjectDisplayName = subjectUserId is not null
            ? await identityService.GetDisplayNameAsync(subjectUserId, ct)
            : null;

        var recipientIds = await context.CookbookMemberships.QueryUserIdsByCookbook(cookbookId, excluded, ct);

        // MemberJoined rule: the newly joined member is not in the member query yet, because the membership
        // row has not been written. Add them when they are missing, so they receive their own join notification.
        if (actionType == CookbookNotificationActionType.MemberJoined && !recipientIds.Contains(actorUserId))
            recipientIds.Add(actorUserId);

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
