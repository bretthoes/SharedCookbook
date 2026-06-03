using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewDefault
{
    private CookbookMembership _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetup() => _actual = CookbookMembership.NewDefault(cookbookId: Guid.NewGuid());

    [Test]
    public void ShouldHaveContributorTier() =>
        Assert.That(_actual.Tier, Is.EqualTo(MembershipTier.Contributor));

    [Test]
    public void ShouldNotBeOwner() => Assert.That(_actual.IsOwner, Is.False);
}

