namespace SharedCookbook.Domain.Entities;

public class CookbookMember : BaseAuditableEntity
{
    public required int PersonId { get; set; }

    public required int CookbookId { get; set; }

    public required bool IsCreator { get; set; }

    public required bool CanAddRecipe { get; set; }

    public required bool CanUpdateRecipe { get; set; }

    public required bool CanDeleteRecipe { get; set; }

    public required bool CanSendInvite { get; set; }

    public required bool CanRemoveMember { get; set; }

    public required bool CanEditCookbookDetails { get; set; }

    public virtual Cookbook? Cookbook { get; set; }
}
