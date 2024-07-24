namespace SharedCookbook.Domain.Entities;

public class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; set; }

    public int? SenderPersonId { get; set; }

    public int? RecipientPersonId { get; set; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; set; }

    public required virtual Cookbook Cookbook { get; set; }
}
