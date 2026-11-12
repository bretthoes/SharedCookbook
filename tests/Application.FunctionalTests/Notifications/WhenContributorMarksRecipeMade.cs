using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;
using SharedCookbook.Application.Recipes.Commands.RecordRecipeMade;
using SharedCookbook.Application.FunctionalTests.Common;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.FunctionalTests.Notifications;

using static Testing;

public class WhenContributorMarksRecipeMade : BaseTestFixture
{
    private int _notificationCount;
    private CookbookNotificationActionType _actionType;
    private bool _ownerNotificationIsImportant;
    private bool _contributorSeesOwnMake;
    private bool _contributorIsActor;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CookbookPermissionScenario.CreateWithContributor();

        await CookbookPermissionScenario.ActAsContributor();
        await SendAsync(new RecordRecipeMadeCommand(context.RecipeId));

        await CookbookPermissionScenario.ActAsOwner();
        var page = await SendAsync(new GetNotificationsWithPaginationQuery());
        _notificationCount = page.Items.Count(n => n.ActionType == CookbookNotificationActionType.RecipeMade);
        var notification = page.Items.First(n => n.ActionType == CookbookNotificationActionType.RecipeMade);
        _actionType = notification.ActionType;
        _ownerNotificationIsImportant = notification.IsImportant;

        await CookbookPermissionScenario.ActAsContributor();
        var ownPage = await SendAsync(new GetNotificationsWithPaginationQuery());
        var ownNotification = ownPage.Items.FirstOrDefault(n => n.ActionType == CookbookNotificationActionType.RecipeMade);
        _contributorSeesOwnMake = ownNotification is not null;
        _contributorIsActor = ownNotification?.ActorIsCurrentUser ?? false;
    }

    [Test]
    public void OwnerHasOneRecipeMadeNotification() => Assert.That(_notificationCount, Is.EqualTo(1));

    [Test]
    public void OwnerNotificationIsRecipeMade() =>
        Assert.That(_actionType, Is.EqualTo(CookbookNotificationActionType.RecipeMade));

    [Test]
    public void OwnerNotificationIsNotImportant() =>
        Assert.That(_ownerNotificationIsImportant, Is.False);

    [Test]
    public void ContributorSeesOwnMake() =>
        Assert.That(_contributorSeesOwnMake, Is.True);

    [Test]
    public void ContributorIsTheActor() =>
        Assert.That(_contributorIsActor, Is.True);
}
