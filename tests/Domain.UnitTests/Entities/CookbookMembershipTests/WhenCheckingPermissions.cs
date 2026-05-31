using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenCheckingPermissions
{
    private const string ActorUserId = "actor-user";
    private const string OtherUserId = "other-user";

    private static CookbookMembership Member(string userId, MembershipTier tier)
    {
        var membership = CookbookMembership.NewDefault(cookbookId: 1, userId: userId);
        membership.SetTier(tier);
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
    public void ContributorShouldUpdateOwnRecipe() =>
        Assert.That(Member(ActorUserId, MembershipTier.Contributor).CanUpdateRecipe(RecipeBy(ActorUserId)), Is.True);

    [Test]
    public void ContributorShouldNotUpdateOtherRecipe() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Contributor).CanUpdateRecipe(RecipeBy(OtherUserId)),
            Is.False);

    [Test]
    public void AdminShouldUpdateOtherRecipe() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Admin).CanUpdateRecipe(RecipeBy(OtherUserId)),
            Is.True);

    [Test]
    public void ContributorShouldDeleteOwnRecipe() =>
        Assert.That(Member(ActorUserId, MembershipTier.Contributor).CanDeleteRecipe(RecipeBy(ActorUserId)), Is.True);

    [Test]
    public void ContributorShouldNotDeleteOtherRecipe() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Contributor).CanDeleteRecipe(RecipeBy(OtherUserId)),
            Is.False);

    [Test]
    public void AdminShouldDeleteOtherRecipe() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Admin).CanDeleteRecipe(RecipeBy(OtherUserId)),
            Is.True);

    [Test]
    public void CanRemoveMemberShouldAllowLeavingOwnMembership() =>
        Assert.That(Member(ActorUserId, MembershipTier.Viewer).CanRemoveMember(Member(ActorUserId, MembershipTier.Viewer)), Is.True);

    [Test]
    public void AdminShouldRemoveContributor() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Admin).CanRemoveMember(Member(OtherUserId, MembershipTier.Contributor)),
            Is.True);

    [Test]
    public void ContributorShouldNotRemoveOtherMember() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Contributor).CanRemoveMember(Member(OtherUserId, MembershipTier.Contributor)),
            Is.False);

    [Test]
    public void OwnerShouldNotUpdateSelfTier() =>
        Assert.That(
            CookbookMembership.NewOwner(ActorUserId).CanApplyTierUpdate(CookbookMembership.NewOwner(ActorUserId), MembershipTier.Admin),
            Is.False);

    [Test]
    public void OwnerShouldUpdateContributorTier() =>
        Assert.That(
            CookbookMembership.NewOwner(ActorUserId).CanApplyTierUpdate(
                Member(OtherUserId, MembershipTier.Contributor),
                MembershipTier.Admin),
            Is.True);

    [Test]
    public void AdminShouldUpdateContributorTier() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Admin).CanApplyTierUpdate(
                Member(OtherUserId, MembershipTier.Contributor),
                MembershipTier.Admin),
            Is.True);

    [Test]
    public void AdminShouldNotUpdateOwnerTier() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Admin).CanApplyTierUpdate(
                CookbookMembership.NewOwner(OtherUserId),
                MembershipTier.Contributor),
            Is.False);

    [Test]
    public void OwnerShouldPromoteContributorToOwner() =>
        Assert.That(
            CookbookMembership.NewOwner(ActorUserId).CanPromoteToOwner(Member(OtherUserId, MembershipTier.Contributor)),
            Is.True);

    [Test]
    public void ContributorShouldNotPromoteToOwner() =>
        Assert.That(
            Member(ActorUserId, MembershipTier.Contributor).CanPromoteToOwner(Member(OtherUserId, MembershipTier.Contributor)),
            Is.False);
}
