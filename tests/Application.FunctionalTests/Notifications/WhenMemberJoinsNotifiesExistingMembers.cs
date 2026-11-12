using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Application.Invitations.Commands.UpdateInvitation;
using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Notifications;

using static Testing;

public class WhenMemberJoinsNotifiesExistingMembers : BaseTestFixture
{
    private CookbookNotificationActionType _ownerActionType;
    private bool _ownerNotificationExists;
    private bool _ownerIsActor;
    private bool _joiningUserReceivedNotification;
    private bool _joiningUserIsSubject;

    [SetUp]
    public async Task SetUp()
    {
        // Owner creates cookbook
        var ownerUserId = await RunAsDefaultUserAsync();
        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: TestData.AnyNonEmptyString));

        // Create another user who will be invited
        await RunAsUserAsync("invitee@test.local", "Testing1234!", []);

        // Owner creates invitation for the invitee
        await RunAsExistingUserAsync("test@local");
        var invitationId = await SendAsync(new CreateInvitationCommand(
            CookbookId: cookbookId,
            Email: "invitee@test.local"));

        // Invitee accepts the invitation (this triggers MembershipCreatedEvent)
        await RunAsExistingUserAsync("invitee@test.local");
        await SendAsync(new UpdateInvitationCommand(invitationId, InvitationStatus.Accepted));

        // Owner checks their notifications
        await RunAsExistingUserAsync("test@local");
        var ownerPage = await SendAsync(new GetNotificationsWithPaginationQuery { PageSize = 50 });
        var ownerNotification = ownerPage.Items.FirstOrDefault(n => n.ActionType == CookbookNotificationActionType.MemberJoined);

        _ownerNotificationExists = ownerNotification is not null;
        _ownerActionType = ownerNotification?.ActionType ?? CookbookNotificationActionType.Unknown;
        _ownerIsActor = ownerNotification?.ActorIsCurrentUser ?? true;

        await RunAsExistingUserAsync("invitee@test.local");
        var inviteePage = await SendAsync(new GetNotificationsWithPaginationQuery { PageSize = 50 });
        var inviteeNotification = inviteePage.Items.FirstOrDefault(n => n.ActionType == CookbookNotificationActionType.MemberJoined);
        _joiningUserReceivedNotification = inviteeNotification is not null;
        _joiningUserIsSubject = inviteeNotification?.SubjectIsCurrentUser ?? false;
    }

    [Test]
    public void OwnerReceivesNotification() => Assert.That(_ownerNotificationExists, Is.True);

    [Test]
    public void OwnerNotificationIsMemberJoined() =>
        Assert.That(_ownerActionType, Is.EqualTo(CookbookNotificationActionType.MemberJoined));

    [Test]
    public void OwnerIsNotTheActor() => Assert.That(_ownerIsActor, Is.False);

    [Test]
    public void JoiningUserSeesOwnJoin() => Assert.That(_joiningUserReceivedNotification, Is.True);

    [Test]
    public void JoiningUserIsTheSubject() => Assert.That(_joiningUserIsSubject, Is.True);
}
