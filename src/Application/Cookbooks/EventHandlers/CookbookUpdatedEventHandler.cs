namespace SharedCookbook.Application.Cookbooks.EventHandlers;

public sealed class CookbookUpdatedEventHandler(ILogger<CookbookUpdatedEventHandler> logger)
    : INotificationHandler<CookbookUpdatedEvent>
{
    public Task Handle(CookbookUpdatedEvent updatedEvent, CancellationToken cancellationToken)
    {
        var cookbook = updatedEvent.Cookbook;

        logger.LogInformation(
            "CookbookUpdatedEvent handled: Cookbook '{Title}' (ID: {Id}) was updated by User ID {UserId}.",
            cookbook.Title,
            cookbook.Id,
            cookbook.LastModifiedBy);
        
        return Task.CompletedTask;
    }
}
