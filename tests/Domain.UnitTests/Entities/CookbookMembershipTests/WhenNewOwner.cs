using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenNewOwner
{
    private CookbookMembership _actual = null!;
    
    [SetUp]
    public void SetUp()
    {
        _actual = CookbookMembership.NewOwner(It.IsAny<string>());
    }
    
    [Test]
    public void ShouldHaveOwnerPermissions()
    {
        var expected = Permissions.Owner;
        
        Assert.That(_actual.Permissions, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldBeOwner() { Assert.That(_actual.IsOwner, Is.True); }
    
    [Test]
    public void AndDemotedThenShouldHaveContributorPermissions()
    {
        var expected = Permissions.Contributor;
        
        _actual.Demote();
        
        Assert.That(_actual.Permissions, Is.EqualTo(expected));
    }
    
    [Test]
    public void AndDemotedThenShouldNotBeOwner()
    {
        _actual.Demote();
        
        Assert.That(_actual.IsOwner, Is.False);
    }
    
    [Test]
    public void AndPromotedThenShouldNotHaveAnyDomainEvents()
    {
        _actual.Promote();
        
        Assert.That(_actual.DomainEvents, Is.Empty);
    }
}
