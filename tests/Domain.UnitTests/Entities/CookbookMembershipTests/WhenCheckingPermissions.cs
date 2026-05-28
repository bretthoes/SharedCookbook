using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenCheckingPermissions
{
    private const string ActorUserId = "actor-user";
    private const string OtherUserId = "other-user";

    private static CookbookMembership Contributor(string userId, Permissions permissions)
    {
        var membership = CookbookMembership.NewDefault(cookbookId: 1, userId: userId);
        membership.SetPermissions(permissions);
        return membership;
    }

    private static Recipe RecipeBy(string userId) => new()
    {
        CreatedBy = userId,
        Title = "Test Recipe",
        Timing = new Timing(null, null, null),
        DietaryTags = new DietaryTags(null, null, null, null, null, null, null, null),
        MealTypes = new MealTypes(null, null, null, null, null)
    };

    [Test]
    public void CanUpdateRecipeShouldAllowOwnRecipeWithoutPermission()
    {
        var actor = Contributor(ActorUserId, Permissions.None);
        var ownRecipe = RecipeBy(ActorUserId);
        var otherRecipe = RecipeBy(OtherUserId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actor.CanUpdateRecipe(ownRecipe), Is.True);
            Assert.That(actor.CanUpdateRecipe(otherRecipe), Is.False);
        }
    }

    [Test]
    public void CanUpdateRecipeShouldAllowPermissionOnOtherRecipe()
    {
        var actor = Contributor(ActorUserId, Permissions.None with { CanUpdateRecipe = true });
        var otherRecipe = RecipeBy(OtherUserId);

        Assert.That(actor.CanUpdateRecipe(otherRecipe), Is.True);
    }

    [Test]
    public void CanDeleteRecipeShouldAllowOwnRecipeWithoutPermission()
    {
        var actor = Contributor(ActorUserId, Permissions.None);
        var ownRecipe = RecipeBy(ActorUserId);
        var otherRecipe = RecipeBy(OtherUserId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actor.CanDeleteRecipe(ownRecipe), Is.True);
            Assert.That(actor.CanDeleteRecipe(otherRecipe), Is.False);
        }
    }

    [Test]
    public void CanDeleteRecipeShouldAllowPermissionOnOtherRecipe()
    {
        var actor = Contributor(ActorUserId, Permissions.None with { CanDeleteRecipe = true });
        var otherRecipe = RecipeBy(OtherUserId);

        Assert.That(actor.CanDeleteRecipe(otherRecipe), Is.True);
    }

    [Test]
    public void CanRemoveMemberShouldAllowLeavingOwnMembership()
    {
        var actor = Contributor(ActorUserId, Permissions.None);

        Assert.That(actor.CanRemoveMember(actor), Is.True);
    }

    [Test]
    public void CanRemoveMemberShouldAllowPermissionOnOtherMembership()
    {
        var actor = Contributor(ActorUserId, Permissions.None with { CanRemoveMember = true });
        var target = Contributor(OtherUserId, Permissions.Contributor);

        Assert.That(actor.CanRemoveMember(target), Is.True);
    }

    [Test]
    public void CanRemoveMemberShouldDenyWithoutPermissionOnOtherMembership()
    {
        var actor = Contributor(ActorUserId, Permissions.Contributor);
        var target = Contributor(OtherUserId, Permissions.Contributor);

        Assert.That(actor.CanRemoveMember(target), Is.False);
    }

    [Test]
    public void CanUpdateMembershipShouldDenyUpdatingSelf()
    {
        var actor = CookbookMembership.NewOwner(ActorUserId);

        Assert.That(actor.CanUpdateMembership(actor), Is.False);
    }

    [Test]
    public void CanUpdateMembershipShouldAllowOwnerToUpdateOtherMember()
    {
        var actor = CookbookMembership.NewOwner(ActorUserId);
        var target = Contributor(OtherUserId, Permissions.Contributor);

        Assert.That(actor.CanUpdateMembership(target), Is.True);
    }

    [Test]
    public void CanUpdateMembershipShouldAllowPermissionOnOtherMember()
    {
        var actor = Contributor(ActorUserId, Permissions.None with { CanRemoveMember = true });
        var target = Contributor(OtherUserId, Permissions.Contributor);

        Assert.That(actor.CanUpdateMembership(target), Is.True);
    }

    [Test]
    public void CanPromoteToOwnerShouldRequireOwnershipAndDifferentMember()
    {
        var owner = CookbookMembership.NewOwner(ActorUserId);
        var target = Contributor(OtherUserId, Permissions.Contributor);
        var contributor = Contributor(ActorUserId, Permissions.Contributor);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(owner.CanPromoteToOwner(target), Is.True);
            Assert.That(owner.CanPromoteToOwner(owner), Is.False);
            Assert.That(contributor.CanPromoteToOwner(target), Is.False);
        }
    }
}
