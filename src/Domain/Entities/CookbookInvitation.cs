namespace SharedCookbook.Domain.Entities;

public sealed class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; init; }

    public string? RecipientPersonId { get; init; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }
    
    public byte[] Hash { get; init; } = []; // 32 bytes SHA-256
    
    public byte[] Salt { get; init; } = [];     // 16 bytes

    public DateTime? ResponseDate { get; set; }

    public Cookbook? Cookbook { get; init; }

    private bool IsAccepted => InvitationStatus == CookbookInvitationStatus.Accepted;
    
    private bool IsRejected => InvitationStatus == CookbookInvitationStatus.Rejected;
    
    // TODO add a computed IsExpired field
    
    public void Accept(DateTime timestamp)
    {
        if (IsAccepted) return;

        InvitationStatus = CookbookInvitationStatus.Accepted;
        ResponseDate = timestamp;
        
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }

    public void Reject(DateTime timestamp)
    {
        if (IsRejected) return;

        InvitationStatus = CookbookInvitationStatus.Rejected;
        ResponseDate = timestamp;
        
        AddDomainEvent(new InvitationRejectedEvent(this));
    }

    public struct Constraints
    {
        public const int HashLength = 32;
        public const int SaltLength = 16;
        public const int InvitationStatusMaxLength = 255;
    }
}
