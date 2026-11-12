namespace SharedCookbook.Application.Common.Interfaces;

public interface INotificationFanOut
{
    /// <summary>
    /// Creates a notification for every member of the cookbook, excluding those based on rules defined
    /// in <see cref="SharedCookbook.Application.Notifications.FanOut.NotificationRecipientRules"/>.
    /// </summary>
    Task FanOutAsync(
        Guid cookbookId,
        string actorUserId,
        CookbookNotificationActionType actionType,
        Guid? recipeId = null,
        string? subjectUserId = null,
        CancellationToken ct = default);
}
