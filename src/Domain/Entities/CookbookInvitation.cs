namespace SharedCookbook.Domain.Entities;

public sealed class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; init; }

    public string? RecipientPersonId { get; init; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; set; }

    public Cookbook? Cookbook { get; init; }
    
    public IReadOnlyCollection<InvitationToken> Tokens { get; init; } = [];

    private bool IsAccepted => InvitationStatus == CookbookInvitationStatus.Accepted;
    
    private bool IsRejected => InvitationStatus == CookbookInvitationStatus.Rejected;
    
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
        public const int InvitationStatusMaxLength = 255;
    }
}
