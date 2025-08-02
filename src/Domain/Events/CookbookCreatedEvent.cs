namespace SharedCookbook.Domain.Events;

public sealed class CookbookCreatedEvent(Cookbook cookbook) : BaseEvent
{
    public Cookbook Cookbook { get; } = cookbook;
}
