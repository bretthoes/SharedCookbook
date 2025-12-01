namespace SharedCookbook.Domain.Common;

public abstract class BaseInvitation : BaseAuditableEntity
{
    public InvitationStatus Status { get; protected set; }
    
    public int CookbookId { get; protected init; }
    
    public DateTimeOffset? ResponseDate { get; private set; }

    public bool IsActive => Status == InvitationStatus.Active;
    
    private bool IsAccepted => Status == InvitationStatus.Accepted;
    
    private bool IsRejected => Status == InvitationStatus.Rejected;
    
    public Cookbook? Cookbook { get; init; }
    
    public void Accept(DateTimeOffset timestamp, string acceptedBy)
    {
        if (IsAccepted) return;
        AddDomainEvent(new InvitationAcceptedEvent(Id, CookbookId, acceptedBy));
        Status = InvitationStatus.Accepted;
        ResponseDate = timestamp;
    }

    public virtual void Reject(DateTimeOffset timestamp)
    {
        if (IsRejected) return;

        Status = InvitationStatus.Rejected;
        ResponseDate = timestamp;
    }

    public struct Constraints
    {
        public const int StatusMaxLength = 255;
    }
}
