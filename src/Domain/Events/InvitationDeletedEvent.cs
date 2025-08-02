namespace SharedCookbook.Domain.Events;

public sealed class InvitationDeletedEvent(CookbookInvitation invitation) : BaseEvent
{
    public CookbookInvitation Invitation { get; } = invitation;
}
