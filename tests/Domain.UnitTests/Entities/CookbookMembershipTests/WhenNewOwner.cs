using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewOwner
{
    private CookbookMembership _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetup() => _actual = CookbookMembership.NewOwner(It.IsAny<string>(), displayName: null);

    [Test]
    public void ShouldHaveOwnerTier() => Assert.That(_actual.Tier, Is.EqualTo(MembershipTier.Owner));

    [Test]
    public void ShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }

    [Test]
    public void TransferringOwnershipShouldDemoteToContributor()
    {
        var owner = CookbookMembership.NewOwner(creatorId: "owner", displayName: null);
        var member = CookbookMembership.New(Guid.NewGuid(), userId: "member", displayName: null, MembershipTier.Contributor);

        owner.TryApplyTierUpdate(member, MembershipTier.Owner);

        Assert.That(owner.Tier, Is.EqualTo(MembershipTier.Contributor));
    }

    [Test]
    public void TransferringOwnershipShouldPromoteTheTarget()
    {
        var owner = CookbookMembership.NewOwner(creatorId: "owner", displayName: null);
        var member = CookbookMembership.New(Guid.NewGuid(), userId: "member", displayName: null, MembershipTier.Contributor);

        owner.TryApplyTierUpdate(member, MembershipTier.Owner);

        Assert.That(member.IsOwner, Is.True);
    }

    [Test]
    public void RefusedTierUpdateShouldReportFailure()
    {
        var actor = CookbookMembership.New(Guid.NewGuid(), userId: "actor", displayName: null, MembershipTier.Contributor);
        var target = CookbookMembership.New(Guid.NewGuid(), userId: "target", displayName: null, MembershipTier.Viewer);

        Assert.That(actor.TryApplyTierUpdate(target, MembershipTier.Admin), Is.False);
    }

    [Test]
    public void RefusedTierUpdateShouldLeaveTheTargetUnchanged()
    {
        var actor = CookbookMembership.New(Guid.NewGuid(), userId: "actor", displayName: null, MembershipTier.Contributor);
        var target = CookbookMembership.New(Guid.NewGuid(), userId: "target", displayName: null, MembershipTier.Viewer);

        actor.TryApplyTierUpdate(target, MembershipTier.Admin);

        Assert.That(target.Tier, Is.EqualTo(MembershipTier.Viewer));
    }
}
