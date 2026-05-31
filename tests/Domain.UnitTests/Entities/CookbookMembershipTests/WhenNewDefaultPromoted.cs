using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewDefaultPromoted
{
    private CookbookMembership _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _actual = CookbookMembership.NewDefault(cookbookId: 1);
        _actual.Promote();
    }

    [Test]
    public void IsPromotedThenDomainEventsShouldNotBeEmpty() { Assert.That(_actual.DomainEvents, Is.Not.Empty); }

    [Test]
    public void IsPromotedThenShouldHaveOwnerTier() => Assert.That(_actual.Tier, Is.EqualTo(MembershipTier.Owner));

    [Test]
    public void IsPromotedThenShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }
}
