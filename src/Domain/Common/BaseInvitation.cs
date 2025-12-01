namespace SharedCookbook.Domain.Common;

public abstract class BaseInvitation : BaseAuditableEntity
{
    public InvitationStatus Status { get; protected set; }
    
    public int CookbookId { get; protected init; }
    
    public DateTimeOffset? ResponseDate { get; private set; }

    protected bool IsActive => Status == InvitationStatus.Active;
    
    public Cookbook? Cookbook { get; init; }
    
    public void Accept(DateTimeOffset timestamp, string acceptedBy)
    {
        if (!IsActive) return;
        AddDomainEvent(new InvitationAcceptedEvent(Id, CookbookId, acceptedBy));
        Status = InvitationStatus.Accepted;
        ResponseDate = timestamp;
    }

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
