using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenInstantiated
{
    [Test]
    public void ShouldHaveNoPermissions()
    {
        var actual = Permissions.None;

        var expected = (new CookbookMembership()).Permissions;
        
        Assert.That(actual, Is.EqualTo(expected));
    }
}
