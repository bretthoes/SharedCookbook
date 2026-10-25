namespace SharedCookbook.Domain.Common;

public abstract class BaseInvitation : BaseAuditableEntity
{
    public InvitationStatus Status { get; protected set; }
    
    public Guid CookbookId { get; protected init; }
    
    public DateTimeOffset? ResponseDate { get; private set; }

    protected bool IsActive => Status == InvitationStatus.Active;
    
    public Cookbook? Cookbook { get; init; }
    
    /// <summary>
    /// Accepts an active invitation and raises <see cref="InvitationAcceptedEvent"/>.
    /// Does nothing when the invitation is not active.
    /// </summary>
    public void Accept(DateTimeOffset timestamp, string acceptedBy)
    {
        if (!IsActive) return;
        AddDomainEvent(new InvitationAcceptedEvent(Id, CookbookId, acceptedBy));
        Status = InvitationStatus.Accepted;
        ResponseDate = timestamp;
    }

    /// <summary>
    /// Rejects an active invitation. Does nothing when the invitation is not active.
    /// Does not raise a domain event.
    /// </summary>
    public virtual void Reject(DateTimeOffset timestamp)
    {
        if (!IsActive) return;

        Status = InvitationStatus.Rejected;
        ResponseDate = timestamp;
    }

    public struct Constraints
    {
        public const int StatusMaxLength = 255;
    }
}
