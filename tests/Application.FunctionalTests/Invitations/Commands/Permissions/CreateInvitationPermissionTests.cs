using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Domain.Entities;
using DomainPermissions = SharedCookbook.Domain.ValueObjects.Permissions;

namespace SharedCookbook.Application.FunctionalTests.Invitations.Commands.Permissions;

using static Testing;
using static Common.CookbookPermissionScenario;

public class WhenContributorLacksSendInvitePermission : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        await EnsureInviteeExists();
        var context = await CreateWithContributor(new DomainPermissions { CanAddRecipe = true });
        _cookbookId = context.CookbookId;
        await ActAsContributor();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new CreateInvitationCommand(_cookbookId, InviteeEmail)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberCreatesInvitation : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        await EnsureInviteeExists();
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsNonMember();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new CreateInvitationCommand(_cookbookId, InviteeEmail)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenOwnerCreatesInvitation : BaseTestFixture
{
    private int _cookbookId;
    private string _inviteeUserId = null!;

    [SetUp]
    public async Task SetUp()
    {
        _inviteeUserId = await EnsureInviteeExists();
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsOwner();
    }

    [Test]
    public async Task ShouldReturnInvitationId() =>
        Assert.That(
            await SendAsync(new CreateInvitationCommand(_cookbookId, InviteeEmail)),
            Is.GreaterThan(0));

    [Test]
    public async Task ShouldCreateInvitationForRecipient()
    {
        await SendAsync(new CreateInvitationCommand(_cookbookId, InviteeEmail));

        Assert.That(
            await AnyAsync<CookbookInvitation>(invitation =>
                invitation.CookbookId == _cookbookId && invitation.RecipientPersonId == _inviteeUserId),
            Is.True);
    }
}

public class WhenContributorCreatesInvitation : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        await EnsureInviteeExists();
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsContributor();
    }

    [Test]
    public async Task ShouldReturnInvitationId() =>
        Assert.That(
            await SendAsync(new CreateInvitationCommand(_cookbookId, InviteeEmail)),
            Is.GreaterThan(0));
}
