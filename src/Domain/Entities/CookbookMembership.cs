using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class CookbookMembership : BaseAuditableEntity
{
    public int CookbookId { get; init; }

    public bool IsOwner { get; private set; }

    public Permissions Permissions { get; private set; } = Permissions.None;

    public Cookbook? Cookbook { get; init; }

    public void MarkDeleted() => AddDomainEvent(new MembershipDeletedEvent(this));

    public void Promote()
    {
        if (IsOwner) return;
        IsOwner = true;
        Permissions = Permissions.Owner;
        AddDomainEvent(new PromotedToOwnerEvent(Id, CookbookId));
    }

    public void Demote()
    {
        if (!IsOwner) return;
        IsOwner = false;
        SetPermissions(Permissions.Contributor);
    }

    public void SetPermissions(Permissions permissions) => Permissions = permissions;

    public static CookbookMembership NewOwner() => new() { IsOwner = true, Permissions = Permissions.Owner };

    public static CookbookMembership NewDefault(int cookbookId) => new()
    {
        CookbookId = cookbookId, IsOwner = false, Permissions = Permissions.Contributor
    };

    public static CookbookMembership NewDefault(int cookbookId, string userId) => new()
    {
        // TODO check if CreatedAt / other intercepted fields are actually being updated?
        CookbookId = cookbookId, IsOwner = false, Permissions = Permissions.Contributor, CreatedBy = userId
    };
}
