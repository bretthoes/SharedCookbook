using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class CookbookMembershipTests
{
    [Test]
    public void NewDefaultMembershipShouldHaveContributorPermissions()
    {
        var expected = Permissions.Contributor;
        
        var actual = CookbookMembership.NewDefault(cookbookId: It.IsAny<int>(), userId: It.IsAny<string>());
        
        actual.Permissions.Should().BeEquivalentTo(expected);
        actual.IsOwner.Should().BeFalse();
    }
    
    [Test]
    public void PromotedMemberShouldBeOwner()
    {
        var expected = Permissions.Owner;
        var sut = CookbookMembership.NewDefault(cookbookId: It.IsAny<int>(), userId: It.IsAny<string>());
        
        sut.Promote();
        
        sut.Permissions.Should().BeEquivalentTo(expected);
        sut.IsOwner.Should().BeFalse();
        sut.DomainEvents.Should().NotBeEmpty();
    }
    
    [Test]
    public void NewOwnerMembershipShouldHaveOwnerPermissions()
    {
        var expected = Permissions.Owner;
        
        var actual = CookbookMembership.NewOwner(It.IsAny<string>());
        
        actual.IsOwner.Should().BeTrue();
        actual.Permissions.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void DemotedOwnerMembershipShouldHaveContributorPermissions()
    {
        var expected = Permissions.Contributor;
        var sut = CookbookMembership.NewOwner(It.IsAny<string>());
        
        sut.Demote();
        
        sut.IsOwner.Should().BeFalse();
        sut.Permissions.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void PromotedOwnerMembershipShouldNotHaveAnyDomainEvents()
    {
        var sut = CookbookMembership.NewOwner(It.IsAny<string>());
        
        sut.Promote();
        
        sut.DomainEvents.Should().BeEmpty();
    }

    [Test]
    public void InstantiatedMembershipShouldHaveNoPermissions()
    {
        var actual = Permissions.None;

        var expected = (new CookbookMembership()).Permissions;
        
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void PermissionsNotOverwritten()
    {
        // TODO clean up; testing for if set is overriding with defaults unexpectedly.
        const bool expected = true;
        var sut = new CookbookMembership();
        var permissions = new Permissions().WithAddRecipe().WithUpdateRecipe(false);
        sut.SetPermissions(permissions);
        var updatedPermissions = permissions.WithUpdateRecipe(false);
        sut.SetPermissions(updatedPermissions);
        sut.Permissions.CanAddRecipe.Should().BeTrue();
    }
}
