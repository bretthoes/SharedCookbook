namespace SharedCookbook.Domain.Events;

public sealed class InvitationAcceptedEvent(CookbookInvitation invitation) : BaseEvent
{
    public CookbookInvitation Invitation { get; } = invitation;
}
