using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities.CookbookMembershipTests;

public class WhenMembershipCapabilities
{
    private static CookbookMembership WithTier(MembershipTier tier, string userId = "user")
    {
        var membership = CookbookMembership.NewDefault(cookbookId: 1, userId: userId);
        membership.SetTier(tier);
        return membership;
    }

    [Test]
    public void ContributorShouldAddRecipeAndSendInvite()
    {
        var member = WithTier(MembershipTier.Contributor);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(member.CanAddRecipe, Is.True);
            Assert.That(member.CanSendInvite, Is.True);
        }
    }

    [Test]
    public void ViewerShouldNotAddRecipeOrSendInvite()
    {
        var member = WithTier(MembershipTier.Viewer);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(member.CanAddRecipe, Is.False);
            Assert.That(member.CanSendInvite, Is.False);
        }
    }

    [Test]
    public void AdminShouldEditCookbookDetails() =>
        Assert.That(WithTier(MembershipTier.Admin).CanEditCookbookDetails, Is.True);

    [Test]
    public void ContributorShouldNotEditCookbookDetails() =>
        Assert.That(WithTier(MembershipTier.Contributor).CanEditCookbookDetails, Is.False);

    [Test]
    public void AdminShouldRemoveContributor() =>
        Assert.That(
            WithTier(MembershipTier.Admin, "admin").CanRemoveMember(WithTier(MembershipTier.Contributor, "target")),
            Is.True);

    [Test]
    public void AdminShouldNotRemoveOwner() =>
        Assert.That(
            WithTier(MembershipTier.Admin, "admin").CanRemoveMember(CookbookMembership.NewOwner("owner", displayName: null)),
            Is.False);

    [Test]
    public void OwnerShouldAssignAnyTierToContributor() =>
        Assert.That(
            CookbookMembership.NewOwner("owner", displayName: null).CanApplyTierUpdate(
                WithTier(MembershipTier.Contributor, "target"),
                MembershipTier.Admin),
            Is.True);

    [Test]
    public void AdminShouldPromoteContributorToAdmin() =>
        Assert.That(
            WithTier(MembershipTier.Admin, "admin").CanApplyTierUpdate(
                WithTier(MembershipTier.Contributor, "target"),
                MembershipTier.Admin),
            Is.True);

    [Test]
    public void AdminShouldDemoteContributorToViewer() =>
        Assert.That(
            WithTier(MembershipTier.Admin, "admin").CanApplyTierUpdate(
                WithTier(MembershipTier.Contributor, "target"),
                MembershipTier.Viewer),
            Is.True);

    [Test]
    public void AdminShouldNotUpdateOtherAdmin() =>
        Assert.That(
            WithTier(MembershipTier.Admin, "admin").CanApplyTierUpdate(
                WithTier(MembershipTier.Admin, "target"),
                MembershipTier.Contributor),
            Is.False);

    [Test]
    public void AdminShouldNotChangeOwnerTier() =>
        Assert.That(
            WithTier(MembershipTier.Admin, "admin").CanApplyTierUpdate(
                CookbookMembership.NewOwner("owner", displayName: null),
                MembershipTier.Contributor),
            Is.False);
}
