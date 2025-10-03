namespace SharedCookbook.Domain.Events;

public sealed class CookbookUpdatedEvent(Cookbook cookbook) : BaseEvent
{
    public Cookbook Cookbook { get; } = cookbook;
}
