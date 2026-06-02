namespace SharedCookbook.Domain.Entities;

/// <summary>
/// A user's membership in a cookbook. Permissions are derived from <see cref="MembershipTier"/>.
/// Tier changes are governed by <see cref="CanApplyTierUpdate"/>.
/// </summary>
public sealed class CookbookMembership : BaseAuditableEntity
{
    public int CookbookId { get; private init; }

    public string? DisplayName { get; set; }

    public MembershipTier Tier { get; private set; } = MembershipTier.Contributor;

    public Cookbook? Cookbook { get; init; }

    public bool IsOwner => Tier == MembershipTier.Owner;

    public bool CanAddRecipe => Tier >= MembershipTier.Contributor;

    public bool CanSendInvite => Tier >= MembershipTier.Contributor;

    public bool CanEditCookbookDetails => Tier >= MembershipTier.Admin;

    public bool CanUpdateRecipe(Recipe recipe) => Tier >= MembershipTier.Admin || IsAuthor(recipe);

    public bool CanDeleteRecipe(Recipe recipe) => Tier >= MembershipTier.Admin || IsAuthor(recipe);

    public bool CanRemoveMember(CookbookMembership target) => IsSameMember(target) || CanRemoveOtherMember(target);

    public bool CanApplyTierUpdate(CookbookMembership target, MembershipTier proposedTier) =>
        !IsSameMember(target) && CanAssignTierTo(target, proposedTier);

    public void Demote() => SetTier(MembershipTier.Contributor);

    public void SetTier(MembershipTier tier) => Tier = tier;

    public static CookbookMembership NewOwner(string creatorId, string? displayName) =>
        new() { Tier = MembershipTier.Owner, CreatedBy = creatorId, DisplayName = displayName };

    public static CookbookMembership NewDefault(int cookbookId, string? userId = null) => new()
    {
        CookbookId = cookbookId,
        Tier = MembershipTier.Contributor,
        CreatedBy = userId
    };

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
            // Owner may set any other member to any tier (including ownership transfer)
            MembershipTier.Owner when proposedTier != target.Tier => true,
            // Admin may change viewers/contributors to any tier below owner (not other admins).
            MembershipTier.Admin when target.Tier is MembershipTier.Contributor or MembershipTier.Viewer
                && proposedTier is MembershipTier.Viewer or MembershipTier.Contributor or MembershipTier.Admin
                && proposedTier != target.Tier => true,
            // Any other role cannot change tier assignment
            _ => false
        };

    private bool IsAuthor(Recipe recipe) => string.Equals(CreatedBy, recipe.CreatedBy, StringComparison.Ordinal);

    private bool IsSameMember(CookbookMembership target) =>
        string.Equals(CreatedBy, target.CreatedBy, StringComparison.Ordinal);
}
