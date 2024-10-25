namespace SharedCookbook.Domain.Events;

public class InvitationDeletedEvent : BaseEvent
{
    public InvitationDeletedEvent(CookbookInvitation invitation)
    {
        Invitation = invitation;
    }

    public CookbookInvitation Invitation { get; }
}
