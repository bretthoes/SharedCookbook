using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewDefaultPromoted
{
    private CookbookMembership _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _actual = CookbookMembership.NewDefault(cookbookId: Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>());
        _actual.SetTier(MembershipTier.Owner);
    }

    [Test]
    public void IsPromotedThenShouldHaveOwnerTier() => Assert.That(_actual.Tier, Is.EqualTo(MembershipTier.Owner));

    [Test]
    public void IsPromotedThenShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }
}

