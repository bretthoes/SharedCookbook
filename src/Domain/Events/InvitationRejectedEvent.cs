namespace SharedCookbook.Domain.Events;

public sealed class InvitationRejectedEvent(CookbookInvitation invitation) : BaseEvent
{
    public CookbookInvitation Invitation { get; } = invitation;
}
