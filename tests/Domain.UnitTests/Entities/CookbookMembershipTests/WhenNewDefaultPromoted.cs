using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class WhenNewDefaultPromoted
{
    private CookbookMembership _actual = null!;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _actual = CookbookMembership.NewDefault(cookbookId: It.IsAny<int>(), userId: It.IsAny<string>());
        
        _actual.Promote();
    }
    
    [Test]
    public void IsPromotedThenDomainEventsShouldNotBeEmpty() { Assert.That(_actual.DomainEvents, Is.Not.Empty); }

    [Test]
    public void IsPromotedThenShouldHaveOwnerPermissions()
    {
        var expected = Permissions.Owner;
        
        Assert.That(_actual.Permissions, Is.EqualTo(expected));
    }
    
    [Test]
    public void IsPromotedThenShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }
}
