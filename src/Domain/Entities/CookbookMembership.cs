using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class CookbookMembership : BaseAuditableEntity
{
    public int CookbookId { get; init; }

    public bool IsOwner { get; private set; }

    public Permissions Permissions { get; private set; } = Permissions.None;

    public Cookbook? Cookbook { get; init; }

    public bool CanDeleteRecipe(Recipe recipe) =>
        string.Equals(CreatedBy, recipe.CreatedBy, StringComparison.Ordinal) || Permissions.CanDeleteRecipe;

    public void Promote()
    {
        if (IsOwner) return;
        IsOwner = true;
        SetPermissions(Permissions.Owner);
        AddDomainEvent(new PromotedToOwnerEvent(Id, CookbookId));
    }

    public void Demote()
    {
        if (!IsOwner) return;
        IsOwner = false;
        SetPermissions(Permissions.Contributor);
    }

    public void SetPermissions(Permissions permissions) => Permissions = permissions;

    public static void TransferOwnershipTo(
        CookbookMembership newOwner,
        IEnumerable<CookbookMembership> departingOwners)
    {
        newOwner.Promote();

        foreach (var owner in departingOwners)
            owner.Demote();
    }

    public static CookbookMembership NewOwner(string creatorId) =>
        new() { IsOwner = true, Permissions = Permissions.Owner, CreatedBy = creatorId };

    public static CookbookMembership NewDefault(int cookbookId, string? userId = null) => new()
    {
        CookbookId = cookbookId, IsOwner = false, Permissions = Permissions.Contributor, CreatedBy = userId
    };
}
