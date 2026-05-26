using SharedCookbook.Application.Memberships.Commands.DeleteMembership;
using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Memberships.Commands.Permissions;

using static Testing;
using static Common.CookbookPermissionScenario;

public class WhenOwnerUpdatesOwnMembership : BaseTestFixture
{
    private int _ownerMembershipId;

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
            () => SendAsync(new UpdateMembershipCommand
            {
                Id = _ownerMembershipId,
                IsOwner = false,
                CanAddRecipe = true,
                CanUpdateRecipe = true,
                CanDeleteRecipe = true,
                CanSendInvite = true,
                CanRemoveMember = true,
                CanEditCookbookDetails = true
            }),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenContributorRemovesMember : BaseTestFixture
{
    private int _ownerMembershipId;

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
    private int _contributorMembershipId;

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
    private int _contributorMembershipId;

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
    private int _contributorMembershipId;

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
    private int _contributorMembershipId;

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
            () => SendAsync(new UpdateMembershipCommand
            {
                Id = _contributorMembershipId,
                IsOwner = false,
                CanAddRecipe = true,
                CanUpdateRecipe = true,
                CanDeleteRecipe = true,
                CanSendInvite = true,
                CanRemoveMember = true,
                CanEditCookbookDetails = true
            }),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberUpdatesMembership : BaseTestFixture
{
    private int _contributorMembershipId;

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
            () => SendAsync(new UpdateMembershipCommand
            {
                Id = _contributorMembershipId,
                IsOwner = false,
                CanAddRecipe = true,
                CanUpdateRecipe = true,
                CanDeleteRecipe = true,
                CanSendInvite = true,
                CanRemoveMember = true,
                CanEditCookbookDetails = true
            }),
            Throws.TypeOf<ForbiddenAccessException>());
}
