using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewOwner
{
    private CookbookMembership _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetup() => _actual = CookbookMembership.NewOwner(It.IsAny<string>());

    [Test]
    public void ShouldHaveOwnerTier() => Assert.That(_actual.Tier, Is.EqualTo(MembershipTier.Owner));

    [Test]
    public void ShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }

    [Test]
    public void AndDemotedThenShouldHaveContributorTier()
    {
        var owner = CookbookMembership.NewOwner(It.IsAny<string>());
        owner.Demote();
        Assert.That(owner.Tier, Is.EqualTo(MembershipTier.Contributor));
    }

    [Test]
    public void AndDemotedThenShouldNotBeOwner()
    {
        var owner = CookbookMembership.NewOwner(It.IsAny<string>());
        owner.Demote();
        Assert.That(owner.IsOwner, Is.False);
    }
}
