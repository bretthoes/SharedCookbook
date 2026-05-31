namespace SharedCookbook.Domain.Events;

public sealed class MembershipCreatedEvent(CookbookMembership membership) : BaseEvent
{
    public CookbookMembership Membership { get; } = membership;
}
