namespace SharedCookbook.Application.Common.Interfaces;

public interface INotificationFanOut
{
    Task FanOutAsync(
        Guid cookbookId,
        string actorUserId,
        CookbookNotificationActionType actionType,
        Guid? recipeId = null,
        string? subjectUserId = null,
        CancellationToken ct = default);
}
