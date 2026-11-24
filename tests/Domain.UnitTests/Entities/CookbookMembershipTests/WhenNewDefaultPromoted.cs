using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewDefaultPromoted
{
    private CookbookMembership _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var owner = CookbookMembership.NewOwner(creatorId: "owner", displayName: null);
        _actual = CookbookMembership.NewDefault(cookbookId: Guid.NewGuid(), userId: "member", displayName: null);
        owner.TryApplyTierUpdate(_actual, MembershipTier.Owner);
    }

    [Test]
    public void IsPromotedThenShouldHaveOwnerTier() => Assert.That(_actual.Tier, Is.EqualTo(MembershipTier.Owner));

    [Test]
    public void IsPromotedThenShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }
}

