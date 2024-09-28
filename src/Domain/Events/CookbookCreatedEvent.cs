namespace SharedCookbook.Domain.Events;

public class CookbookCreatedEvent : BaseEvent
{
    public CookbookCreatedEvent(Cookbook cookbook)
    {
        Cookbook = cookbook;
    }

    public Cookbook Cookbook { get; }
}
