namespace SharedCookbook.Application.Cookbooks.EventHandlers;

public class CookbookUpdatedEventHandler(ILogger<CookbookUpdatedEventHandler> logger)
    : INotificationHandler<CookbookUpdatedEvent>
{
    public Task Handle(CookbookUpdatedEvent deletedEvent, CancellationToken cancellationToken)
    {
        var cookbook = deletedEvent.Cookbook;

        logger.LogInformation("CookbookUpdatedEvent handled: Cookbook '{Title}' (ID: {Id}) was updated by User ID {UserId}.",
            cookbook.Title,
            cookbook.Id,
            cookbook.LastModifiedBy);
        
        return Task.CompletedTask;
    }
}
