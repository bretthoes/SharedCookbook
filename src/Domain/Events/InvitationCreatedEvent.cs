namespace SharedCookbook.Domain.Events;

public class InvitationCreatedEvent : BaseEvent
{
    public InvitationCreatedEvent(CookbookInvitation invitation)
    {
        Invitation = invitation;
    }

    public CookbookInvitation Invitation { get; }
}
