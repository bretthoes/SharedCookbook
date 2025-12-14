using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class CookbookTests
{
    [Test]
    public void CreatedCookbookShouldHaveOwner()
    {
        var expected = CookbookMembership.NewOwner(It.IsAny<string>()).Permissions;
        var sut = Cookbook.Create(title: It.IsAny<string>(), creatorId: It.IsAny<string>());
        
        var actual = sut.Memberships.Single();
        
        Assert.That(actual.Permissions, Is.EqualTo(expected));
    }

    [Test]
    public void CreatedCookbookShouldHaveExpectedTitle()
    {
        const string expected = "Cookbook";
        
        var actual = Cookbook.Create(title: expected, creatorId: It.IsAny<string>());
        
        Assert.That(actual.Title, Is.EqualTo(expected));
    }

    [Test]
    public void DefaultCreatedCookbookShouldHaveNullImage()
    {
        var actual = Cookbook.Create(title: It.IsAny<string>(), creatorId: It.IsAny<string>());
        
        Assert.That(actual.Image, Is.Null);
    }

    [Test]
    public void CreatedCookbookWithImageShouldHaveExpectedImage()
    {
        var expected = Guid.NewGuid().ToString();
        
        var actual  = Cookbook.Create(title: It.IsAny<string>(), creatorId: It.IsAny<string>(), image: expected);
        
        Assert.That(actual.Image, Is.EqualTo(expected));
    }

    [Test]
    public void InstantiatedCookbookShouldNotHaveOwner()
    {
        var actual = new Cookbook { Title = It.IsAny<string>() };
        
        Assert.That(actual.Memberships, Is.Empty);
    }
}
