namespace SharedCookbook.Domain.Events;

public sealed class CookbookDeletedEvent(Cookbook cookbook) : BaseEvent
{
    public Cookbook Cookbook { get; } = cookbook;
}
