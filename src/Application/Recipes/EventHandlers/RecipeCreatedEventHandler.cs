namespace SharedCookbook.Application.Recipes.EventHandlers;

public sealed class RecipeCreatedEventHandler(INotificationFanOut fanOut, IUser user)
    : INotificationHandler<RecipeCreatedEvent>
{
    public Task Handle(RecipeCreatedEvent notification, CancellationToken ct = default)
    {
        var recipe = notification.Recipe;
        var actorUserId = recipe.CreatedBy ?? user.Id;

        if (string.IsNullOrWhiteSpace(actorUserId))
            return Task.CompletedTask;

        return fanOut.FanOutAsync(
            recipe.CookbookId,
            actorUserId,
            CookbookNotificationActionType.NewRecipe,
            recipe.Id,
            ct: ct);
    }
}
