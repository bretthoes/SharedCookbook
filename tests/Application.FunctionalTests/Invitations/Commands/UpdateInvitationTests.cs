using SharedCookbook.Application.Invitations.Commands.UpdateInvitation;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.FunctionalTests.Invitations.Commands;

using static Testing;

[TestFixture]
public class InvitationAcceptanceTests : BaseTestFixture
{
    [Test]
    public async Task AcceptingInvitation_UpdatesStatusAndResponseDate()
    {
        string userId = await RunAsDefaultUserAsync();
        var cookbook = WithActiveInvitation("Test", userId);
        await AddAsync(cookbook);

        int invitationId = await SendAsync(new UpdateInvitationCommand(cookbook.Id, InvitationStatus.Accepted));

        var invitation = await FindAsync<CookbookInvitation>(invitationId);
        invitation.Should().NotBeNull();
        invitation.Status.Should().Be(InvitationStatus.Accepted);
        invitation.CookbookId.Should().Be(cookbook.Id);
        invitation.ResponseDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task AcceptingInvitation_CreatesMembershipForRecipient()
    {
        string userId = await RunAsDefaultUserAsync();
        var cookbook = WithActiveInvitation("Test", userId);
        await AddAsync(cookbook);

        await SendAsync(new UpdateInvitationCommand(cookbook.Id, InvitationStatus.Accepted));

        var membership = await SingleAsync<CookbookMembership>(m =>
            m.CookbookId == cookbook.Id && m.CreatedBy == userId);
        membership.Should().NotBeNull();
        membership.CreatedBy.Should().Be(userId);
        membership.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task RejectingInvitation_DoesNotCreateMembership()
    {
        string userId = await RunAsDefaultUserAsync();
        var cookbook = WithActiveInvitation("Test", userId);
        await AddAsync(cookbook);

        await SendAsync(new UpdateInvitationCommand(cookbook.Id, InvitationStatus.Rejected));

        bool any = await AnyAsync<CookbookMembership>(m =>
            m.CookbookId == cookbook.Id && m.CreatedBy == userId);
        any.Should().BeFalse();
    }

    private static Cookbook WithActiveInvitation(string title, string recipientUserId) => new()
    {
        Title = title, Invitations = [CookbookInvitation.Create(cookbookId: 0, recipientUserId)]
    };
}
