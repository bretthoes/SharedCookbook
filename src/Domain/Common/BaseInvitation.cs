namespace SharedCookbook.Domain.Common;

public abstract class BaseInvitation : BaseAuditableEntity
{
    public required InvitationStatus Status { get; set; }
    
    public required int CookbookId { get; set; }
    
    public DateTime? ResponseDate { get; set; }
    
    public bool IsActive => Status == InvitationStatus.Active;
    
    private bool IsAccepted => Status == InvitationStatus.Accepted;
    
    private bool IsRejected => Status == InvitationStatus.Rejected;
    
    public Cookbook? Cookbook { get; init; }
    
    public virtual void Accept(DateTime timestamp)
    {
        if (IsAccepted) return;

        Status = InvitationStatus.Accepted;
        ResponseDate = timestamp;
    }

    public virtual void Reject(DateTime timestamp)
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
