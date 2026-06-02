using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.FunctionalTests.Memberships.Commands;

using static Testing;

public class WhenOwnerPromotesContributorToOwner : BaseTestFixture
{
    private const string ContributorEmail = "contributor@test.com";
    private const string ContributorPassword = "Testing1234!";
    private const string CookbookTitle = "Existing Cookbook";

    private string _originalOwnerUserId = null!;
    private string _contributorUserId = null!;
    private CookbookMembership _demotedOwner = null!;
    private CookbookMembership _promotedOwner = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();

        _contributorUserId = await RunAsUserAsync(ContributorEmail, ContributorPassword, []);
        var contributorMembership = new CookbookMembership { CreatedBy = _contributorUserId };

        _originalOwnerUserId = await RunAsDefaultUserAsync();
        var ownerMembership = CookbookMembership.NewOwner(_originalOwnerUserId, It.IsAny<string>());

        await AddAsync(new Cookbook
        {
            Title = CookbookTitle,
            CreatedBy = _originalOwnerUserId,
            Memberships = [ownerMembership, contributorMembership],
        });

        await SendAsync(new UpdateMembershipCommand(contributorMembership.Id, MembershipTier.Owner));

        var memberships = await ListAsync<CookbookMembership>();
        _demotedOwner = memberships.Single(m => m.CreatedBy == _originalOwnerUserId);
        _promotedOwner = memberships.Single(m => m.CreatedBy == _contributorUserId);
    }

    [Test]
    public void ShouldDemoteOriginalOwner() =>
        Assert.That(_demotedOwner.Tier, Is.EqualTo(MembershipTier.Contributor));

    [Test]
    public void ShouldPromoteContributor() =>
        Assert.That(_promotedOwner.Tier, Is.EqualTo(MembershipTier.Owner));

    [Test]
    public void ShouldRecordDemotionModifiedBy() =>
        Assert.That(_demotedOwner.LastModifiedBy, Is.EqualTo(_originalOwnerUserId));

    [Test]
    public void ShouldRecordPromotionModifiedBy() =>
        Assert.That(_promotedOwner.LastModifiedBy, Is.EqualTo(_originalOwnerUserId));
}
