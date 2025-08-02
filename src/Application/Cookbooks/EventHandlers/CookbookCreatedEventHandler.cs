namespace SharedCookbook.Application.Cookbooks.EventHandlers;

public class CookbookCreatedEventHandler(ILogger<CookbookCreatedEventHandler> logger)
    : INotificationHandler<CookbookCreatedEvent>
{
    public Task Handle(CookbookCreatedEvent acceptedEvent, CancellationToken cancellationToken)
    {
        var cookbook = acceptedEvent.Cookbook;

        logger.LogInformation("CookbookCreatedEvent handled: Cookbook '{Title}' (ID: {Id}) was created by User ID {UserId}.",
            cookbook.Title,
            cookbook.Id,
            cookbook.CreatedBy);
        
        return Task.CompletedTask;
    }
}
