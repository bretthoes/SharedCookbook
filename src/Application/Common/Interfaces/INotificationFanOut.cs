namespace SharedCookbook.Application.Common.Interfaces;

public interface INotificationFanOut
{
    Task FanOutAsync(
        int cookbookId,
        string actorUserId,
        CookbookNotificationActionType actionType,
        int? recipeId = null,
        string? subjectUserId = null,
        CancellationToken ct = default);
}
