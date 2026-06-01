using SharedCookbook.Domain.Enums;
using SharedCookbook.Infrastructure.Identity.Projections;

namespace SharedCookbook.Infrastructure.UnitTests.Identity;

public class NotificationProjectionsTests
{
    private const string RecipientId = "recipient";
    private const string OtherUserId = "other";

    [TestCase(CookbookNotificationActionType.MemberJoined)]
    [TestCase(CookbookNotificationActionType.MemberLeft)]
    [TestCase(CookbookNotificationActionType.MemberRemoved)]
    [TestCase(CookbookNotificationActionType.NewRecipe)]
    public void MembershipActionsAreImportant(CookbookNotificationActionType actionType) =>
        Assert.That(
            NotificationProjections.IsImportantForRecipient(actionType, RecipientId, recipeCreatedBy: null),
            Is.True);

    [Test]
    public void RecipeMadeIsImportantWhenRecipientAddedTheRecipe() =>
        Assert.That(
            NotificationProjections.IsImportantForRecipient(
                CookbookNotificationActionType.RecipeMade,
                RecipientId,
                RecipientId),
            Is.True);

    [Test]
    public void RecipeMadeIsNotImportantWhenSomeoneElseAddedTheRecipe() =>
        Assert.That(
            NotificationProjections.IsImportantForRecipient(
                CookbookNotificationActionType.RecipeMade,
                RecipientId,
                OtherUserId),
            Is.False);
}
