namespace SharedCookbook.Domain.Entities;

public class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; init; }

    public string? RecipientPersonId { get; init; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; init; }

    public Cookbook? Cookbook { get; init; }
    
    public void Accept()
    {
        if (InvitationStatus == CookbookInvitationStatus.Accepted) return;

        InvitationStatus = CookbookInvitationStatus.Accepted;
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }

    public struct Constraints
    {
        public const int InvitationStatusMaxLength = 255;
    }
}
