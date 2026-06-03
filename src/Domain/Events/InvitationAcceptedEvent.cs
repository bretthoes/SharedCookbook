namespace SharedCookbook.Domain.Events;

public class InvitationAcceptedEvent(Guid invitationId, Guid cookbookId, string userId) : BaseEvent
{
    public Guid InvitationId { get; } = invitationId;
    public Guid CookbookId { get; } = cookbookId;
    public string UserId { get; } = userId;
}
