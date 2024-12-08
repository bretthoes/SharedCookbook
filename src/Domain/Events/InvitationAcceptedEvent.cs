namespace SharedCookbook.Domain.Events;

public class InvitationAcceptedEvent(CookbookInvitation invitation) : BaseEvent
{
    public CookbookInvitation Invitation { get; } = invitation;
}
