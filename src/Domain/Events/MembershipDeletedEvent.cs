namespace SharedCookbook.Domain.Events;

public sealed class MembershipDeletedEvent(CookbookMembership membership) : BaseEvent
{
    public CookbookMembership Membership { get; } = membership;
}
