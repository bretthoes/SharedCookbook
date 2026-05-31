namespace SharedCookbook.Application.Recipes.EventHandlers;

public class RecipeUpdatedEventHandler(ILogger<RecipeUpdatedEventHandler> logger, IUser user)
    : INotificationHandler<RecipeUpdatedEvent>
{
    public Task Handle(RecipeUpdatedEvent notification, CancellationToken ct = default)
    {
        logger.LogInformation("Recipe {Id} was updated by User {UserId}.", notification.RecipeId, user.Id);

        return Task.CompletedTask;
    }
}
