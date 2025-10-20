namespace SharedCookbook.Domain.Events;

public sealed class MembershipUpdatedEvent(CookbookMembership membership) : BaseEvent
{
    public CookbookMembership Membership { get; } = membership;
}
