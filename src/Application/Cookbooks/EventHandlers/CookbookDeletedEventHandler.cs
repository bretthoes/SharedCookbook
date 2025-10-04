namespace SharedCookbook.Application.Cookbooks.EventHandlers;

public class CookbookDeletedEventHandler(ILogger<CookbookDeletedEventHandler> logger)
    : INotificationHandler<CookbookDeletedEvent>
{
    public Task Handle(CookbookDeletedEvent deletedEvent, CancellationToken cancellationToken)
    {
        var cookbook = deletedEvent.Cookbook;

        logger.LogInformation(
            "CookbookUpdatedEvent handled: Cookbook '{Title}' (ID: {Id}) was updated by User ID {UserId}.",
            cookbook.Title,
            cookbook.Id,
            cookbook.LastModifiedBy);
        
        return Task.CompletedTask;
    }
}
