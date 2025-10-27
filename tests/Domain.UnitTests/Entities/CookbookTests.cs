using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class CookbookTests
{
    [Test]
    public void CreatedCookbookShouldHaveOwner()
    {
        var expected = CookbookMembership.NewOwner(It.IsAny<string>()).Permissions;
        var sut = Cookbook.Create(title: It.IsAny<string>(), creatorId: It.IsAny<string>(), image: It.IsAny<string>());
        
        var actual = sut.Memberships.Single();
        
        actual.Permissions.Should().BeEquivalentTo(expected);
    }
}
