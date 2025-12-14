using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class WhenNewDefault
{
    private CookbookMembership _actual = null!;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _actual = CookbookMembership.NewDefault(cookbookId: It.IsAny<int>(), userId: It.IsAny<string>());
    }
    
    [Test]
    public void ShouldHaveContributorPermissions()
    {
        var expected = Permissions.Contributor;
        
        Assert.That(_actual.Permissions, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldNotBeOwner() { Assert.That(_actual.IsOwner, Is.False); }
}
