using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.ValueObjects;

public class PermissionsTests
{
    [Test]
    public void NonePermissionsShouldAllBeFalse()
    {
        var actual = Permissions.None;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.CanAddRecipe, Is.False);
            Assert.That(actual.CanDeleteRecipe, Is.False);
            Assert.That(actual.CanEditCookbookDetails, Is.False);
            Assert.That(actual.CanRemoveMember, Is.False);
            Assert.That(actual.CanSendInvite, Is.False);
            Assert.That(actual.CanUpdateRecipe, Is.False);
        }
    }
    
    [Test]
    public void OwnerPermissionsShouldAllBeTrue()
    {
        var actual = Permissions.Owner;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.CanAddRecipe, Is.True);
            Assert.That(actual.CanDeleteRecipe, Is.True);
            Assert.That(actual.CanEditCookbookDetails, Is.True);
            Assert.That(actual.CanRemoveMember, Is.True);
            Assert.That(actual.CanSendInvite, Is.True);
            Assert.That(actual.CanUpdateRecipe, Is.True);
        }
    }
    
    [Test]
    public void ContributorPermissionsShouldAllBeExpected()
    {
        var actual = Permissions.Contributor;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.CanAddRecipe, Is.True);
            Assert.That(actual.CanDeleteRecipe, Is.False);
            Assert.That(actual.CanEditCookbookDetails, Is.False);
            Assert.That(actual.CanRemoveMember, Is.False);
            Assert.That(actual.CanSendInvite, Is.True);
            Assert.That(actual.CanUpdateRecipe, Is.False);
        }
    }
}
