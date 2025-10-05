namespace SharedCookbook.Domain.Events;

public class PromotedToOwnerEvent(CookbookMembership membership) : BaseEvent
{
    public CookbookMembership Membership { get; } = membership;
}
