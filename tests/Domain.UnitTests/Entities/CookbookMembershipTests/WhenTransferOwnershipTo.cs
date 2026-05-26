using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenTransferOwnershipTo
{
    private CookbookMembership _newOwner = null!;
    private CookbookMembership _departingOwner = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _departingOwner = CookbookMembership.NewOwner("original-owner");
        _newOwner = CookbookMembership.NewDefault(cookbookId: 1, userId: "new-owner");

        CookbookMembership.TransferOwnershipTo(_newOwner, [_departingOwner]);
    }

    [Test]
    public void ShouldPromoteNewOwner() =>
        Assert.That(_newOwner.IsOwner, Is.True);

    [Test]
    public void ShouldDemoteDepartingOwner() =>
        Assert.That(_departingOwner.IsOwner, Is.False);

    [Test]
    public void ShouldGiveNewOwnerOwnerPermissions() =>
        Assert.That(_newOwner.Permissions, Is.EqualTo(Permissions.Owner));

    [Test]
    public void ShouldGiveDepartingOwnerContributorPermissions() =>
        Assert.That(_departingOwner.Permissions, Is.EqualTo(Permissions.Contributor));

    [Test]
    public void ShouldRaisePromotedToOwnerEventOnNewOwner() =>
        Assert.That(_newOwner.DomainEvents, Is.Not.Empty);
}
