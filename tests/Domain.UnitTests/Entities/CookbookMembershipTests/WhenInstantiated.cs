using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenInstantiated
{
    [Test]
    public void ShouldDefaultToContributorTier() =>
        Assert.That(new CookbookMembership().Tier, Is.EqualTo(MembershipTier.Contributor));
}
