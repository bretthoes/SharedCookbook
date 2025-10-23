using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.ValueObjects;

public class PermissionsTests
{
    [Test]
    public void NonePermissionsShouldAllBeFalse()
    {
        var actual = Permissions.None;

        actual.CanAddRecipe.Should().BeFalse();
        actual.CanDeleteRecipe.Should().BeFalse();
        actual.CanEditCookbookDetails.Should().BeFalse();
        actual.CanRemoveMember.Should().BeFalse();
        actual.CanSendInvite.Should().BeFalse();
        actual.CanUpdateRecipe.Should().BeFalse();
    }
    
    [Test]
    public void OwnerPermissionsShouldAllBeTrue()
    {
        var actual = Permissions.Owner;

        actual.CanAddRecipe.Should().BeTrue();
        actual.CanDeleteRecipe.Should().BeTrue();
        actual.CanEditCookbookDetails.Should().BeTrue();
        actual.CanRemoveMember.Should().BeTrue();
        actual.CanSendInvite.Should().BeTrue();
        actual.CanUpdateRecipe.Should().BeTrue();
    }
    
    [Test]
    public void ContributorPermissionsShouldAllBeExpected()
    {
        var actual = Permissions.Contributor;

        actual.CanAddRecipe.Should().BeTrue();
        actual.CanDeleteRecipe.Should().BeFalse();
        actual.CanEditCookbookDetails.Should().BeFalse();
        actual.CanRemoveMember.Should().BeFalse();
        actual.CanSendInvite.Should().BeTrue();
        actual.CanUpdateRecipe.Should().BeFalse();
    }
}
