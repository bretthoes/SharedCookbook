using SharedCookbook.Application.Invitations.Commands.UpdateInvitation;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.FunctionalTests.Invitations.Commands;

using static Testing;

public class InvitationAcceptanceTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public async Task AcceptingInvitation_UpdatesStatusAndResponseDate()
    {
        string? userId = GetUserId();
        var cookbook = WithActiveInvitation("Test", userId!);
        await AddAsync(cookbook);

        var invitation = await SingleAsync<CookbookInvitation>(i =>
            i.CookbookId == cookbook.Id && i.RecipientPersonId == userId);
        int invitationId = await SendAsync(new UpdateInvitationCommand(invitation.Id, InvitationStatus.Accepted));

        var updatedInvitation = await FindAsync<CookbookInvitation>(invitationId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedInvitation, Is.Not.Null);
            Assert.That(updatedInvitation!.Status, Is.EqualTo(InvitationStatus.Accepted));
            Assert.That(updatedInvitation.CookbookId, Is.EqualTo(cookbook.Id));
            Assert.That(updatedInvitation.ResponseDate, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(10)));
        }
    }

    [Test]
    public async Task AcceptingInvitation_CreatesMembershipForRecipient()
    {
        string? userId = GetUserId();
        var cookbook = WithActiveInvitation("Test", userId!);
        await AddAsync(cookbook);

        var invitation = await SingleAsync<CookbookInvitation>(i =>
            i.CookbookId == cookbook.Id && i.RecipientPersonId == userId);
        await SendAsync(new UpdateInvitationCommand(invitation.Id, InvitationStatus.Accepted));

        var membership = await SingleAsync<CookbookMembership>(m =>
            m.CookbookId == cookbook.Id && m.CreatedBy == userId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(membership, Is.Not.Null);
            Assert.That(membership.CreatedBy, Is.EqualTo(userId));
            Assert.That(membership.Created, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(10)));
        }
    }

    [Test]
    public async Task RejectingInvitation_DoesNotCreateMembership()
    {
        string? userId = GetUserId();
        var cookbook = WithActiveInvitation("Test", userId!);
        await AddAsync(cookbook);

        var invitation = await SingleAsync<CookbookInvitation>(i =>
            i.CookbookId == cookbook.Id && i.RecipientPersonId == userId);
        await SendAsync(new UpdateInvitationCommand(invitation.Id, InvitationStatus.Rejected));

        bool any = await AnyAsync<CookbookMembership>(m =>
            m.CookbookId == cookbook.Id && m.CreatedBy == userId);
        Assert.That(any, Is.False);
    }

    private static Cookbook WithActiveInvitation(string title, string recipientUserId) => new()
    {
        Title = title, Invitations = [CookbookInvitation.Create(cookbookId: 0, recipientUserId)]
    };
}
