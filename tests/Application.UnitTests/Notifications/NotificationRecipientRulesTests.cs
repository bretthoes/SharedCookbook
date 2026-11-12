using SharedCookbook.Application.Notifications.FanOut;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.UnitTests.Notifications;

public class NotificationRecipientRulesTests
{
    [Test]
    public void MemberRemovedSkipsTheRemovedMember()
    {
        var skip = NotificationRecipientRules.UserIdsToSkip(
            CookbookNotificationActionType.MemberRemoved,
            "removed-member");

        Assert.That(skip, Is.EquivalentTo(new[] { "removed-member" }));
    }

    [Test]
    public void MemberLeftSkipsTheMemberWhoLeft()
    {
        var skip = NotificationRecipientRules.UserIdsToSkip(
            CookbookNotificationActionType.MemberLeft,
            "member-who-left");

        Assert.That(skip, Is.EquivalentTo(new[] { "member-who-left" }));
    }

    [TestCase(CookbookNotificationActionType.MemberJoined)]
    [TestCase(CookbookNotificationActionType.NewRecipe)]
    [TestCase(CookbookNotificationActionType.RecipeMade)]
    public void OtherActivitiesSkipNobody(CookbookNotificationActionType actionType)
    {
        var skip = NotificationRecipientRules.UserIdsToSkip(actionType, "someone");

        Assert.That(skip, Is.Empty);
    }
}
