namespace SharedCookbook.Domain.Entities;

public class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; init; }

    public string? RecipientPersonId { get; init; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; set; }

    public Cookbook? Cookbook { get; init; }

    public bool IsNotAccepted => !IsAccepted;

    private bool IsAccepted => InvitationStatus == CookbookInvitationStatus.Accepted;
    
    public void Accept(DateTime timestamp)
    {
        if (IsAccepted) return;

        InvitationStatus = CookbookInvitationStatus.Accepted;
        ResponseDate = timestamp;
        
        // AddDomainEvent(new InvitationAcceptedEvent(this));
    }

    public struct Constraints
    {
        public const int InvitationStatusMaxLength = 255;
    }
}
