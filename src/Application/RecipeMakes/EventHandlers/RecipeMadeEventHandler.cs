namespace SharedCookbook.Application.RecipeMakes.EventHandlers;

public sealed class RecipeMadeEventHandler(INotificationFanOut fanOut)
    : INotificationHandler<RecipeMadeEvent>
{
    public Task Handle(RecipeMadeEvent notification, CancellationToken ct = default) =>
        fanOut.FanOutAsync(
            notification.CookbookId,
            notification.ActorUserId,
            CookbookNotificationActionType.RecipeMade,
            notification.RecipeId,
            ct: ct);
}
