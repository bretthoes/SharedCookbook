namespace SharedCookbook.Domain.Entities;

public class CookbookMember : BaseAuditableEntity
{
    public int CookbookId { get; set; }

    public required bool IsCreator { get; set; }

    public required bool CanAddRecipe { get; set; }

    public required bool CanUpdateRecipe { get; set; }

    public required bool CanDeleteRecipe { get; set; }

    public required bool CanSendInvite { get; set; }

    public required bool CanRemoveMember { get; set; }

    public required bool CanEditCookbookDetails { get; set; }

    public Cookbook? Cookbook { get; set; }
    
    public static CookbookMember GetNewCreatorMembership()
    {
        return new()
        {
            IsCreator = true,
            CanAddRecipe = true,
            CanDeleteRecipe = true,
            CanEditCookbookDetails = true,
            CanRemoveMember = true,
            CanSendInvite = true,
            CanUpdateRecipe = true,
        };
    }

    public static CookbookMember GetDefaultMembership(int cookbookId)
    {
        return new()
        {
            CookbookId = cookbookId,
            IsCreator = false,
            CanAddRecipe = true,
            CanUpdateRecipe = false,
            CanDeleteRecipe = false,
            CanSendInvite = true,
            CanRemoveMember = false,
            CanEditCookbookDetails = false
        };
    }
}
