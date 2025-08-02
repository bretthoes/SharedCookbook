namespace SharedCookbook.Domain.Events;

public sealed class InvitationCreatedEvent(CookbookInvitation invitation) : BaseEvent
{
    public CookbookInvitation Invitation { get; } = invitation;
}
