using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class CookbookTests
{
    [Test]
    public void CreatedCookbookShouldHaveOwner()
    {
        var expected = CookbookMembership.NewOwner(It.IsAny<string>()).Permissions;
        var sut = Cookbook.Create(title: TestData.AnyNonEmptyString, creatorId: It.IsAny<string>());
        
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
        var actual = Cookbook.Create(title: TestData.AnyNonEmptyString, creatorId: It.IsAny<string>());
        
        Assert.That(actual.Image, Is.Null);
    }

    [Test]
    public void CreatedCookbookWithImageShouldHaveExpectedImage()
    {
        var expected = Guid.NewGuid().ToString();
        
        var actual  = Cookbook.Create(title: TestData.AnyNonEmptyString, creatorId: It.IsAny<string>(), image: expected);
        
        Assert.That(actual.Image, Is.EqualTo(expected));
    }

    [Test]
    public void InstantiatedCookbookShouldNotHaveOwner()
    {
        var actual = new Cookbook { Title = TestData.AnyNonEmptyString };
        
        Assert.That(actual.Memberships, Is.Empty);
    }

    [Test]
    public void TitleExceedingConstraintShouldThrow()
    {
        string veryLongString = new(c: TestData.AnyNonEmptyChar, count: Cookbook.Constraints.TitleMaxLength + 1);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Cookbook.Create(title: veryLongString, It.IsAny<string>()));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void EmptyTitleShouldThrow(string title)
    {
        Assert.Throws<ArgumentException>(() => Cookbook.Create(title, It.IsAny<string>()));
    }

    [Test]
    public void NullTitleShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => Cookbook.Create(null!, It.IsAny<string>()));
    }

    [Test]
    public void ImageExceedingConstraintShouldThrow()
    {
        string veryLongString = new(c: TestData.AnyNonEmptyChar, count: Cookbook.Constraints.ImageMaxLength + 1);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Cookbook.Create(title: TestData.AnyNonEmptyString, It.IsAny<string>(), image: veryLongString));
    }
}
