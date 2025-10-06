namespace SharedCookbook.Domain.Events;

using MediatR;

public class InvitationAcceptedEvent(int invitationId, int cookbookId) : BaseEvent
{
    public int InvitationId { get; } = invitationId;
    public int CookbookId { get; } = cookbookId;
}
