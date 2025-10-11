namespace SharedCookbook.Domain.Events;

public class InvitationAcceptedEvent(int invitationId, int cookbookId, string userId) : BaseEvent
{
    public int InvitationId { get; } = invitationId;
    public int CookbookId { get; } = cookbookId;
    public string UserId { get; } = userId;
}
