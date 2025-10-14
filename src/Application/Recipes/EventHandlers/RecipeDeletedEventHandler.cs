namespace SharedCookbook.Application.Recipes.EventHandlers;

public class RecipeDeletedEventHandler(ILogger<RecipeDeletedEventHandler> logger, IUser user)
    : INotificationHandler<RecipeDeletedEvent>
{
    public Task Handle(RecipeDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Recipe {Id} was deleted by User {UserId}.", notification.RecipeId, user.Id);

        return Task.CompletedTask;
    }
}
