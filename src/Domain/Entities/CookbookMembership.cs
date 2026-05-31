namespace SharedCookbook.Domain.Entities;

public sealed class CookbookMembership : BaseAuditableEntity
{
    public int CookbookId { get; init; }

    public MembershipTier Tier { get; private set; } = MembershipTier.Contributor;

    public Cookbook? Cookbook { get; init; }

    public bool IsOwner => Tier == MembershipTier.Owner;

    public bool CanAddRecipe() => Tier >= MembershipTier.Contributor;

    public bool CanSendInvite() => Tier >= MembershipTier.Contributor;

    public bool CanEditCookbookDetails() => Tier >= MembershipTier.Admin;

    public bool CanUpdateRecipe(Recipe recipe) => CanUpdateAnyRecipe() || IsAuthor(recipe);

    public bool CanDeleteRecipe(Recipe recipe) => CanDeleteAnyRecipe() || IsAuthor(recipe);

    public bool CanRemoveMember(CookbookMembership target) =>
        IsSameMember(target) || CanRemoveOtherMember(target);

    public bool CanApplyTierUpdate(CookbookMembership target, MembershipTier proposedTier) =>
        !IsSameMember(target) && CanAssignTierTo(target, proposedTier);

    public bool CanPromoteToOwner(CookbookMembership target) =>
        IsOwner && !IsSameMember(target) && !target.IsOwner;

    public void Promote()
    {
        if (IsOwner) return;
        SetTier(MembershipTier.Owner);
        AddDomainEvent(new PromotedToOwnerEvent(Id, CookbookId));
    }

    public void Demote() => SetTier(MembershipTier.Contributor);

    public void SetTier(MembershipTier tier) => Tier = tier;

    public static CookbookMembership NewOwner(string creatorId) =>
        new() { Tier = MembershipTier.Owner, CreatedBy = creatorId };

    public static CookbookMembership NewDefault(int cookbookId, string? userId = null) => new()
    {
        CookbookId = cookbookId,
        Tier = MembershipTier.Contributor,
        CreatedBy = userId
    };

    private bool CanUpdateAnyRecipe() => Tier >= MembershipTier.Admin;

    private bool CanDeleteAnyRecipe() => Tier >= MembershipTier.Admin;

    private bool CanRemoveOtherMember(CookbookMembership target) =>
        Tier switch
        {
            MembershipTier.Owner => target.Tier < MembershipTier.Owner,
            MembershipTier.Admin => target.Tier is MembershipTier.Contributor or MembershipTier.Viewer,
            _ => false
        };

    private bool CanAssignTierTo(CookbookMembership target, MembershipTier proposedTier) =>
        Tier switch
        {
            MembershipTier.Owner when proposedTier != target.Tier => true,
            MembershipTier.Admin when target.Tier is MembershipTier.Contributor or MembershipTier.Viewer
                && proposedTier is MembershipTier.Contributor or MembershipTier.Admin => true,
            _ => false
        };

    private bool IsAuthor(Recipe recipe) =>
        string.Equals(CreatedBy, recipe.CreatedBy, StringComparison.Ordinal);

    private bool IsSameMember(CookbookMembership target) =>
        string.Equals(CreatedBy, target.CreatedBy, StringComparison.Ordinal);
}
