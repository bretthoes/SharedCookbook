using SharedCookbook.Application.Memberships.Commands.DeleteMembership;
using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.FunctionalTests.Memberships.Commands.Permissions;

using static Testing;
using static Common.CookbookPermissionScenario;

public class WhenOwnerUpdatesOwnMembership : BaseTestFixture
{
    private Guid _ownerMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _ownerMembershipId = context.OwnerMembershipId;
        await ActAsOwner();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateMembershipCommand(_ownerMembershipId, MembershipTier.Admin)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenContributorRemovesMember : BaseTestFixture
{
    private Guid _ownerMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _ownerMembershipId = context.OwnerMembershipId;
        await ActAsContributor();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new DeleteMembershipCommand(_ownerMembershipId)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenContributorLeavesCookbook : BaseTestFixture
{
    private Guid _contributorMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _contributorMembershipId = context.ContributorMembershipId;
        await ActAsContributor();
    }

    [Test]
    public async Task ShouldDeleteContributorMembership()
    {
        await SendAsync(new DeleteMembershipCommand(_contributorMembershipId));

        Assert.That(await FindAsync<CookbookMembership>(_contributorMembershipId), Is.Null);
    }
}

public class WhenOwnerRemovesMember : BaseTestFixture
{
    private Guid _contributorMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _contributorMembershipId = context.ContributorMembershipId;
        await ActAsOwner();
    }

    [Test]
    public async Task ShouldDeleteContributorMembership()
    {
        await SendAsync(new DeleteMembershipCommand(_contributorMembershipId));

        Assert.That(await FindAsync<CookbookMembership>(_contributorMembershipId), Is.Null);
    }
}

public class WhenNonMemberRemovesMembership : BaseTestFixture
{
    private Guid _contributorMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _contributorMembershipId = context.ContributorMembershipId;
        await ActAsNonMember();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new DeleteMembershipCommand(_contributorMembershipId)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenContributorUpdatesMembership : BaseTestFixture
{
    private Guid _contributorMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _contributorMembershipId = context.ContributorMembershipId;
        await ActAsContributor();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateMembershipCommand(_contributorMembershipId, MembershipTier.Admin)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberUpdatesMembership : BaseTestFixture
{
    private Guid _contributorMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _contributorMembershipId = context.ContributorMembershipId;
        await ActAsNonMember();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateMembershipCommand(_contributorMembershipId, MembershipTier.Admin)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenAdminPromotesContributorToAdmin : BaseTestFixture
{
    private const string AdminEmail = "admin@test.local";
    private Guid _contributorMembershipId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _contributorMembershipId = context.ContributorMembershipId;

        var adminUserId = await RunAsUserAsync(AdminEmail, ContributorPassword, []);
        var adminMembership = CookbookMembership.NewDefault(context.CookbookId, adminUserId);
        adminMembership.SetTier(MembershipTier.Admin);
        await AddAsync(adminMembership);

        await RunAsExistingUserAsync(AdminEmail);
    }

    [Test]
    public async Task ShouldSetContributorTierToAdmin()
    {
        await SendAsync(new UpdateMembershipCommand(_contributorMembershipId, MembershipTier.Admin));

        var updated = await FindAsync<CookbookMembership>(_contributorMembershipId);
        Assert.That(updated!.Tier, Is.EqualTo(MembershipTier.Admin));
    }
}
